using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class SavedExam : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public string PriorityLevel { get; set; } = "High"; // High, Medium, Low
    public string? PersonalNotes { get; set; }
    public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
}
