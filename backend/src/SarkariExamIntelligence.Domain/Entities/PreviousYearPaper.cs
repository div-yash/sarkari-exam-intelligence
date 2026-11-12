using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class PreviousYearPaper : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int CycleYear { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PaperUrl { get; set; } = string.Empty;
    public string SourceLabel { get; set; } = string.Empty;
    public bool IsOfficial { get; set; } = true;
}
