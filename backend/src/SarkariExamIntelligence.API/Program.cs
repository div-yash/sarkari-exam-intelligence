using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using SarkariExamIntelligence.API.Middleware;
using SarkariExamIntelligence.API.Services;
using SarkariExamIntelligence.Application;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Infrastructure;
using SarkariExamIntelligence.Infrastructure.Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
var notificationsUploadPath = Path.Combine(wwwrootPath, "notifications");
Directory.CreateDirectory(notificationsUploadPath);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/sarkari-exam-api-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sarkari Exam Intelligence API",
        Version = "v1",
        Description = "Production Indian Government Exam Discovery, Eligibility, Notification & Preparation Intelligence API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var configured = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var csvEnv = builder.Configuration["CORS_ALLOWED_ORIGINS"]
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? Array.Empty<string>();

        var origins = configured
            .Concat(csvEnv)
            .Concat(new[] { "http://localhost:4200", "https://localhost:4200" })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sarkari Exam Intelligence API v1"));
}

var fileProvider = new PhysicalFileProvider(wwwrootPath);
if (File.Exists(Path.Combine(wwwrootPath, "index.html")))
{
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider
});
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    service = "Sarkari Exam Intelligence API",
    status = "Running",
    health = "/api/health",
    swagger = "/swagger"
}));

if (File.Exists(Path.Combine(wwwrootPath, "index.html")))
{
    app.MapFallback(async ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api") || ctx.Request.Path.StartsWithSegments("/swagger"))
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }
        ctx.Response.ContentType = "text/html; charset=utf-8";
        await ctx.Response.SendFileAsync(Path.Combine(wwwrootPath, "index.html"));
    });
}

if (!app.Configuration.GetValue<bool>("SkipSeeding"))
{
    const int maxRetries = 6;
    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await DatabaseSeeder.SeedAsync(app.Services);
            break;
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            app.Logger.LogWarning(ex, "Database migration/seed attempt {Attempt}/{MaxRetries} failed. Retrying in 5s...", attempt, maxRetries);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

app.Run();

public partial class Program { }
