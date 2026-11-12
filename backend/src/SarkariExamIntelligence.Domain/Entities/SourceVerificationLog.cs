using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class SourceVerificationLog : BaseEntity
{
    public Guid OfficialSourceId { get; set; }
    public OfficialSource OfficialSource { get; set; } = null!;

    public Guid? ExamNotificationId { get; set; }
    public ExamNotification? ExamNotification { get; set; }

    public DateTime CheckedAtUtc { get; set; } = DateTime.UtcNow;
    public string CheckMethod { get; set; } = "Manual"; // Manual, ScheduledHttpCheck, AdminUpload
    public int? HttpStatusCode { get; set; }
    public string? ContentHash { get; set; }
    public bool ChangesDetected { get; set; }
    public VerificationOutcome Outcome { get; set; } = VerificationOutcome.ManualVerificationRequired;
    public string? Notes { get; set; }
    public string? VerifiedByUserId { get; set; }
}
