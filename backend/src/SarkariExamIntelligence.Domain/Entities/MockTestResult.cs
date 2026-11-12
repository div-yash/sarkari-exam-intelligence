using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class MockTestResult : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public string TestTitle { get; set; } = string.Empty;
    public DateOnly TestDate { get; set; }
    public decimal ScoreObtained { get; set; }
    public decimal MaxScore { get; set; }
    public decimal AccuracyPercent { get; set; }
    public int TimeTakenMinutes { get; set; }
    public string? WeakAreasNotes { get; set; }
}
