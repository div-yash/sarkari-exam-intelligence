using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class ExamPattern : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public string StageName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int? QuestionsCount { get; set; }
    public decimal? MaxMarks { get; set; }
    public int? DurationMinutes { get; set; }
    public string? NegativeMarkingDetails { get; set; }
    public bool IsQualifyingOnly { get; set; }
    public int OrderIndex { get; set; }
}
