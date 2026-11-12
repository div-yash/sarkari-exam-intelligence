using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class CategoryWiseVacancy : BaseEntity
{
    public Guid ExamVacancyId { get; set; }
    public ExamVacancy ExamVacancy { get; set; } = null!;

    public CategoryType Category { get; set; }
    public string? SubCategory { get; set; }
    public int? VacancyCount { get; set; }
    public string? Notes { get; set; }
}
