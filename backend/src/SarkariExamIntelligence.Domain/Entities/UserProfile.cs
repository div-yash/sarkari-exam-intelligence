using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public CategoryType Category { get; set; } = CategoryType.General;
    public bool IsPwBD { get; set; }
    public bool IsExServiceman { get; set; }
    public string DomicileState { get; set; } = string.Empty;
    public string GraduationQualification { get; set; } = string.Empty;
    public string? EngineeringBranch { get; set; }
    public string? University { get; set; }
    public decimal? GraduationPercentage { get; set; }
    public decimal? GraduationCgpa { get; set; }
    public int? PassingYear { get; set; }
    public int WorkExperienceMonths { get; set; }
    public bool IsWorkingProfessional { get; set; }
    public string? CurrentOrganizationType { get; set; }
    public string? AdditionalQualifications { get; set; }
    public bool HasValidGateScore { get; set; }
    public string? GateSubject { get; set; }
    public int? GateScore { get; set; }
    public int? GateYear { get; set; }
    public bool HasBiharDomicileCertificate { get; set; }
    public bool HasValidNclOrEwsCertificate { get; set; }
    public string Nationality { get; set; } = "Indian";
}
