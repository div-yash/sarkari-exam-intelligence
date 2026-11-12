using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Approve, Postpone, Cancel, VerifySource
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? Reason { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
