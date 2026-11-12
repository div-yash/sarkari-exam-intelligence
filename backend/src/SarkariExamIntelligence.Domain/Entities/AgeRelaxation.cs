using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class AgeRelaxation : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public CategoryType Category { get; set; }
    public string? SubCategory { get; set; } // e.g., PwBD-UR, PwBD-SC/ST, Female, Ex-Serviceman, Bihar Govt Employee
    public int RelaxationYears { get; set; }
    public int? MaxUpperAgeOverride { get; set; }
    public bool RequiresDomicileForStateQuota { get; set; }
    public bool IsSupportedByNotification { get; set; } = true;
    public string Conditions { get; set; } = string.Empty;
    public string? SourceReference { get; set; }
}
