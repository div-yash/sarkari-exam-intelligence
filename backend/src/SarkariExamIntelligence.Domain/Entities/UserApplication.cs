using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class UserApplication : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public Guid? ExamNotificationId { get; set; }
    public ExamNotification? ExamNotification { get; set; }

    public ApplicationTrackingStatus TrackingStatus { get; set; } = ApplicationTrackingStatus.Interested;
    public DateOnly? AppliedDate { get; set; }
    public string? ApplicationReferenceNumber { get; set; }
    public string DocumentsChecklistJson { get; set; } = "[]";
    public string? Notes { get; set; }
}
