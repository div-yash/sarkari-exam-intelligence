using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class ExamVacancy : BaseEntity
{
    public Guid ExamNotificationId { get; set; }
    public ExamNotification ExamNotification { get; set; } = null!;

    public int? TotalVacancies { get; set; }
    public int? ItSpecificVacancies { get; set; }
    public int? GeneralVacancies { get; set; }
    public VacancyType VacancyType { get; set; } = VacancyType.NotAnnounced;
    public string? Notes { get; set; }
    public string? SourceReference { get; set; }

    public ICollection<CategoryWiseVacancy> CategoryWiseVacancies { get; set; } = new List<CategoryWiseVacancy>();
}
