using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class OfficialSource : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public ExamOrganization Organization { get; set; } = null!;

    public string SourceName { get; set; } = string.Empty;
    public string OfficialSourceUrl { get; set; } = string.Empty;
    public string? CalendarOrFeedUrl { get; set; }
    public string? LastNotificationPdfUrl { get; set; }
    public DateTime? DateLastCheckedUtc { get; set; }
    public DateTime? DateLastUpdatedUtc { get; set; }
    public DateOnly? SourcePublicationDate { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.VerifiedOfficial;
    public DataConfidence DataConfidence { get; set; } = DataConfidence.ConfirmedOfficial;
    public int? LastHttpStatusCode { get; set; }
    public string? LastContentHash { get; set; }
    public int RateLimitSeconds { get; set; } = 60;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<SourceVerificationLog> VerificationLogs { get; set; } = new List<SourceVerificationLog>();
}
