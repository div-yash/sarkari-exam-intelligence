using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class ExamDate : BaseEntity
{
    public Guid ExamNotificationId { get; set; }
    public ExamNotification ExamNotification { get; set; } = null!;

    public CalendarEventType EventType { get; set; }
    public string Label { get; set; } = string.Empty;
    public DateOnly? ConfirmedDate { get; set; }
    public DateOnly? ExpectedDate { get; set; }
    public bool IsExpected { get; set; }
    public string DisplayStatus { get; set; } = "Not Announced"; // Confirmed, Expected, Not Announced, Postponed, Cancelled
    public string? Notes { get; set; }
}
