using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.DTOs.Eligibility;

public class EligibilityCheckResultDto
{
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamSlug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public ExamStatus CurrentStatus { get; set; }
    public DateOnly? ApplicationEndDate { get; set; }
    public bool IsBiharSpecific { get; set; }
    public bool IsItSpecific { get; set; }

    public EligibilityResultStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public AgeEvaluationDto AgeEvaluation { get; set; } = new();
    public IReadOnlyList<EligibilityCriterionCheckDto> CriteriaChecks { get; set; } = Array.Empty<EligibilityCriterionCheckDto>();
    public IReadOnlyList<string> Reasons { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> VerificationActionsRequired { get; set; } = Array.Empty<string>();
}

public class AgeEvaluationDto
{
    public DateOnly? CandidateDob { get; set; }
    public DateOnly? CutoffDateUsed { get; set; }
    public bool IsCutoffDateTentative { get; set; }
    public string CutoffDateBasis { get; set; } = string.Empty;
    public string CalculatedAgeText { get; set; } = "Not Calculated";
    public decimal? CalculatedAgeYearsDecimal { get; set; }
    public int BaseMinAge { get; set; }
    public int? BaseMaxAge { get; set; }
    public int AppliedRelaxationYears { get; set; }
    public int? EffectiveMaxAge { get; set; }
    public string RelaxationExplanation { get; set; } = string.Empty;
    public bool PassesAgeRule { get; set; }
}

public class EligibilityCriterionCheckDto
{
    public string Criterion { get; set; } = string.Empty; // Age, Qualification, Branch, Marks, Experience, Nationality, Domicile, BiharReservation, GateScore, Gender
    public bool Passed { get; set; }
    public bool RequiresVerification { get; set; }
    public string Detail { get; set; } = string.Empty;
}

public class EligibilitySummaryDto
{
    public int TotalExamsEvaluated { get; set; }
    public int EligibleCount { get; set; }
    public int PotentiallyEligibleCount { get; set; }
    public int NotEligibleCount { get; set; }
    public IReadOnlyList<EligibilityCheckResultDto> Results { get; set; } = Array.Empty<EligibilityCheckResultDto>();
}

public class AdHocEligibilityProfileRequest
{
    public DateOnly? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public CategoryType Category { get; set; } = CategoryType.General;
    public bool IsPwBD { get; set; }
    public bool IsExServiceman { get; set; }
    public string DomicileState { get; set; } = "Bihar";
    public string GraduationQualification { get; set; } = "B.Tech";
    public string? EngineeringBranch { get; set; } = "Information Technology";
    public decimal? GraduationPercentage { get; set; }
    public decimal? GraduationCgpa { get; set; }
    public int? PassingYear { get; set; }
    public int WorkExperienceMonths { get; set; }
    public string? AdditionalQualifications { get; set; }
    public bool HasValidGateScore { get; set; }
    public string? GateSubject { get; set; }
    public int? GateScore { get; set; }
    public bool HasBiharDomicileCertificate { get; set; }
    public bool HasValidNclOrEwsCertificate { get; set; }
    public string Nationality { get; set; } = "Indian";
}
