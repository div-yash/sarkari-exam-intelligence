using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class EligibilityRule : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public Guid? ExamNotificationId { get; set; }
    public ExamNotification? ExamNotification { get; set; }

    public int MinAgeYears { get; set; } = 18;
    public int? MaxAgeYears { get; set; } // Null means no upper age limit (e.g., GATE)
    public DateOnly? AgeCutoffDate { get; set; }
    public string AgeCutoffBasisDescription { get; set; } = "As per official notification cutoff date";

    public string MinimumQualificationLevel { get; set; } = "Graduate"; // 10th, 12th, Diploma, Graduate, BTechBE, PostGraduate
    public bool RequiresEngineeringOrTechDegree { get; set; }
    public bool AcceptsAnyGraduateDegree { get; set; }
    public string? AcceptedEngineeringBranchesCsv { get; set; } // e.g. "Information Technology,Computer Science,CSE,ECE"
    public string QualificationSummary { get; set; } = string.Empty;

    public decimal? MinPercentageGeneral { get; set; }
    public decimal? MinPercentageReserved { get; set; }
    public decimal? MinCgpa { get; set; }
    public bool FinalYearAppearingEligible { get; set; }

    public bool RequiresWorkExperience { get; set; }
    public int MinWorkExperienceMonths { get; set; }
    public string? ExperienceRequirementNotes { get; set; }

    public string NationalityRequirement { get; set; } = "Citizen of India";
    public string? DomicileRequirement { get; set; }
    public bool BiharDomicileRequiredForReservation { get; set; }
    public string? BiharCertificateConditions { get; set; }

    public string? GenderRequirement { get; set; } // Null = All, Male, Female, Unmarried
    public bool RequiresValidGateScore { get; set; }
    public string? AcceptedGatePapersCsv { get; set; }
    public int? MinGateScore { get; set; }

    public string? AdditionalConditions { get; set; }
    public string? VerificationNotes { get; set; }
    public string? SourceReference { get; set; }
}
