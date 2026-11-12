using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; set; }
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
