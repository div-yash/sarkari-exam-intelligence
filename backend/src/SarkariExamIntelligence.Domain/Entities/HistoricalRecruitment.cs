using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class HistoricalRecruitment : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int CycleYear { get; set; }
    public HistoricalDataStatus DataStatus { get; set; } = HistoricalDataStatus.Verified;
    public DateOnly? NotificationDate { get; set; }
    public DateOnly? ApplicationStartDate { get; set; }
    public DateOnly? ApplicationEndDate { get; set; }
    public DateOnly? PrelimExamDate { get; set; }
    public DateOnly? MainsExamDate { get; set; }
    public DateOnly? ResultDate { get; set; }
    public int? TotalVacancies { get; set; }
    public int? ItVacancies { get; set; }
    public int? GeneralVacancies { get; set; }
    public string? SelectionPatternSummary { get; set; }
    public string? OfficialSourceUrl { get; set; }
    public string? Notes { get; set; }

    public ICollection<HistoricalCategoryVacancy> CategoryVacancies { get; set; } = new List<HistoricalCategoryVacancy>();
    public ICollection<PreviousYearCutoff> Cutoffs { get; set; } = new List<PreviousYearCutoff>();
}
