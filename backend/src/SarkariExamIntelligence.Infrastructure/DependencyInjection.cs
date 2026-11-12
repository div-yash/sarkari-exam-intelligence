using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Infrastructure.Identity;
using SarkariExamIntelligence.Infrastructure.Persistence;
using SarkariExamIntelligence.Infrastructure.Services;

namespace SarkariExamIntelligence.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        var provider = ResolveProvider(configuration["DatabaseProvider"], rawConnectionString);
        var normalizedConnectionString = provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase)
            ? NormalizePostgreSqlConnectionString(rawConnectionString)
            : rawConnectionString;

        services.AddDbContext<AppDbContext>(options =>
        {
            if (provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                options.UseInMemoryDatabase(normalizedConnectionString);
            }
            else if (provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(normalizedConnectionString, npgsql =>
                {
                    npgsql.EnableRetryOnFailure(3);
                });
            }
            else
            {
                options.UseSqlServer(normalizedConnectionString, sql =>
                {
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure(3);
                });
            }
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        var jwtKey = configuration["Jwt:Key"] ?? "SarkariExamIntelligence_Dev_Secret_Key_ChangeInProd_2026!";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "SarkariExamIntelligence";
        var jwtAudience = configuration["Jwt:Audience"] ?? "SarkariExamIntelligenceClient";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

        services.AddHttpClient("OfficialSourceVerifier", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(12);
        });

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IEmailSender, EmailSenderService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<ISourceVerificationService, SourceVerificationService>();

        var disableJobs = configuration.GetValue<bool>("DisableBackgroundJobs");
        if (!disableJobs)
        {
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("NotificationDeadlineAlertJob");
                q.AddJob<QuartzNotificationCheckJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("NotificationDeadlineAlertTrigger")
                    .StartAt(DateBuilder.FutureDate(15, IntervalUnit.Second))
                    .WithSimpleSchedule(x => x.WithIntervalInHours(6).RepeatForever()));
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        return services;
    }

    public static string ResolveProvider(string? configuredProvider, string connectionString)
    {
        if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            || connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        {
            return "PostgreSQL";
        }

        if (!string.IsNullOrWhiteSpace(configuredProvider))
            return configuredProvider;

        return "SqlServer";
    }

    public static string NormalizePostgreSqlConnectionString(string raw)
    {
        var trimmed = raw.Trim();
        if (!trimmed.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !trimmed.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        var uri = new Uri(trimmed);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var database = uri.AbsolutePath.TrimStart('/');
        var port = uri.Port > 0 ? uri.Port : 5432;

        return $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
}
