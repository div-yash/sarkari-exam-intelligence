using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class HistoricalCategoryVacancy : BaseEntity
{
    public Guid HistoricalRecruitmentId { get; set; }
    public HistoricalRecruitment HistoricalRecruitment { get; set; } = null!;

    public CategoryType Category { get; set; }
    public int? VacancyCount { get; set; }
    public string? Notes { get; set; }
}
