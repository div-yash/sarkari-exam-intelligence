using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class UserStudyPlan : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public string PlanTitle { get; set; } = string.Empty;
    public int DailyTargetMinutes { get; set; } = 120;
    public int WeeklyTargetHours { get; set; } = 15;
    public DateOnly? TargetExamDate { get; set; }
    public string TimetableJson { get; set; } = "[]";
    public bool IsActive { get; set; } = true;
}
