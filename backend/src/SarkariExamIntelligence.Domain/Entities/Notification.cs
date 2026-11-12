using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? ExamId { get; set; }
    public Exam? Exam { get; set; }
    public Guid? ExamNotificationId { get; set; }
    public ExamNotification? ExamNotification { get; set; }

    public NotificationType NotificationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string DeduplicationKey { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public bool EmailSent { get; set; }
    public DateTime? EmailSentAtUtc { get; set; }
    public DateTime ScheduledForUtc { get; set; } = DateTime.UtcNow;
}
