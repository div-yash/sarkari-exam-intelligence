namespace SarkariExamIntelligence.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly TodayIst { get; }
}

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, string fullName, IEnumerable<string> roles);
    DateTime GetExpiryUtc();
}

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

public interface IAuditService
{
    Task LogAsync(string entityName, string entityId, string action, object? oldValues, object? newValues, string? reason = null, CancellationToken cancellationToken = default);
}
