using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class PreviousYearCutoff : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public Guid? HistoricalRecruitmentId { get; set; }
    public HistoricalRecruitment? HistoricalRecruitment { get; set; }

    public int CycleYear { get; set; }
    public string StageName { get; set; } = string.Empty;
    public CategoryType Category { get; set; }
    public decimal? CutoffMarks { get; set; }
    public decimal? TotalMaxMarks { get; set; }
    public string? SourceUrl { get; set; }
    public string? Notes { get; set; }
}
