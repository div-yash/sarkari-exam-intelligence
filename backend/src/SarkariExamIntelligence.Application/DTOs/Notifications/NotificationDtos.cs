using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.DTOs.Notifications;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid? ExamId { get; set; }
    public string? ExamName { get; set; }
    public string? ExamSlug { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public bool EmailSent { get; set; }
    public DateTime ScheduledForUtc { get; set; }
}

public class NotificationPreferenceDto
{
    public bool EnableInApp { get; set; }
    public bool EnableEmail { get; set; }
    public bool AlertOnNewNotification { get; set; }
    public bool AlertOnApplicationOpen { get; set; }
    public bool Alert30DaysBefore { get; set; }
    public bool Alert7DaysBefore { get; set; }
    public bool Alert3DaysBefore { get; set; }
    public bool Alert1DayBefore { get; set; }
    public bool AlertOnExamDateChange { get; set; }
    public bool AlertOnAdmitCard { get; set; }
    public bool AlertOnResult { get; set; }
    public string? OverrideEmail { get; set; }
}

public class UpdateNotificationPreferenceRequest
{
    public bool EnableInApp { get; set; } = true;
    public bool EnableEmail { get; set; } = true;
    public bool AlertOnNewNotification { get; set; } = true;
    public bool AlertOnApplicationOpen { get; set; } = true;
    public bool Alert30DaysBefore { get; set; } = true;
    public bool Alert7DaysBefore { get; set; } = true;
    public bool Alert3DaysBefore { get; set; } = true;
    public bool Alert1DayBefore { get; set; } = true;
    public bool AlertOnExamDateChange { get; set; } = true;
    public bool AlertOnAdmitCard { get; set; } = true;
    public bool AlertOnResult { get; set; } = true;
    public string? OverrideEmail { get; set; }
}
