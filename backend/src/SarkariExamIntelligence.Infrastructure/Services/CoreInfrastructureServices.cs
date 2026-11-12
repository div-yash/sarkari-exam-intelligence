using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Domain.Entities;

namespace SarkariExamIntelligence.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    private static readonly TimeZoneInfo IstZone = ResolveIstZone();

    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly TodayIst
    {
        get
        {
            var istNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IstZone);
            return DateOnly.FromDateTime(istNow);
        }
    }

    private static TimeZoneInfo ResolveIstZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"); }
        catch
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata"); }
            catch { return TimeZoneInfo.Utc; }
        }
    }
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DateTime GetExpiryUtc()
    {
        var minutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var m) ? m : 720;
        return DateTime.UtcNow.AddMinutes(minutes);
    }

    public string GenerateToken(Guid userId, string email, string fullName, IEnumerable<string> roles)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "SarkariExamIntelligence";
        var audience = _configuration["Jwt:Audience"] ?? "SarkariExamIntelligenceClient";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: GetExpiryUtc(),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AuditService(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task LogAsync(
        string entityName,
        string entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            UserId = _currentUser.UserId?.ToString(),
            UserEmail = _currentUser.Email ?? "system",
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            OldValuesJson = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
            NewValuesJson = newValues is null ? null : JsonSerializer.Serialize(newValues),
            Reason = reason,
            TimestampUtc = DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

public class EmailSenderService : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var smtpHost = _configuration["Email:SmtpHost"];
        if (string.IsNullOrWhiteSpace(smtpHost))
        {
            _logger.LogInformation("SMTP not configured. Simulated email delivery to {Recipient} | Subject: {Subject}", toEmail, subject);
            return;
        }

        var port = int.TryParse(_configuration["Email:SmtpPort"], out var p) ? p : 587;
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];
        var fromEmail = _configuration["Email:FromAddress"] ?? "alerts@sarkariexamintelligence.in";
        var enableSsl = !bool.TryParse(_configuration["Email:EnableSsl"], out var ssl) || ssl;

        using var client = new SmtpClient(smtpHost, port)
        {
            EnableSsl = enableSsl,
            Credentials = string.IsNullOrWhiteSpace(username)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(username, password)
        };

        using var message = new MailMessage(fromEmail, toEmail, subject, htmlBody)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
