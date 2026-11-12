using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.DTOs.Exams;

public class ExamFilterRequest
{
    public string? Search { get; set; }
    public string? CategorySlug { get; set; }
    public string? State { get; set; }
    public string? Qualification { get; set; }
    public string? EngineeringBranch { get; set; }
    public int? MaxAgeLimit { get; set; }
    public ExamStatus? Status { get; set; }
    public int? NotificationYear { get; set; }
    public bool? OnlyOpenApplications { get; set; }
    public bool? NotificationsExpectedThisMonth { get; set; }
    public bool? NotificationsExpectedIn3Months { get; set; }
    public bool? ExamsIn30Days { get; set; }
    public bool? ExamsIn90Days { get; set; }
    public bool? OnlyBihar { get; set; }
    public bool? OnlyItSpecific { get; set; }
    public bool? OnlyGraduateLevel { get; set; }
    public bool? OnlyMatchingMyEligibility { get; set; }
    public string SortBy { get; set; } = "deadline"; // deadline, notificationDate, examDate, vacancies, eligibility
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ExamListItemDto
{
    public Guid Id { get; set; }
    public Guid? CurrentNotificationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobDesignation { get; set; } = string.Empty;
    public GroupClassification GroupClassification { get; set; }
    public JobNature JobNature { get; set; }
    public bool IsBiharSpecific { get; set; }
    public bool IsItSpecific { get; set; }
    public bool IsGraduateLevel { get; set; }

    public int? CycleYear { get; set; }
    public ExamStatus Status { get; set; }
    public DateOnly? NotificationDate { get; set; }
    public DateOnly? ExpectedNotificationDate { get; set; }
    public bool IsNotificationDateExpected { get; set; }
    public DateOnly? ApplicationStartDate { get; set; }
    public DateOnly? ExpectedApplicationStartDate { get; set; }
    public bool IsApplicationStartExpected { get; set; }
    public DateOnly? ApplicationEndDate { get; set; }
    public DateOnly? ExpectedApplicationEndDate { get; set; }
    public bool IsApplicationEndExpected { get; set; }
    public DateOnly? ExamDate { get; set; }
    public DateOnly? ExpectedExamDate { get; set; }
    public bool IsExamDateExpected { get; set; }

    public int? DaysToDeadline { get; set; }
    public int? TotalVacancies { get; set; }
    public int? ItSpecificVacancies { get; set; }
    public VacancyType VacancyType { get; set; }

    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string QualificationSummary { get; set; } = string.Empty;

    public VerificationStatus VerificationStatus { get; set; }
    public DataConfidence DataConfidence { get; set; }
    public string OfficialRecruitmentWebsite { get; set; } = string.Empty;
    public string? OfficialNotificationPdfUrl { get; set; }
    public string? OfficialApplicationUrl { get; set; }

    public bool IsSaved { get; set; }
    public EligibilityResultStatus? UserEligibilityStatus { get; set; }
    public string? UserEligibilitySummary { get; set; }
}

public class ExamDetailDto
{
    // Core Identity (Fields 1-6)
    public Guid Id { get; set; }
    public Guid? CurrentNotificationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public string ConductingOrganization { get; set; } = string.Empty;
    public string OrganizationShortCode { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobDesignation { get; set; } = string.Empty;
    public GroupClassification GroupClassification { get; set; }
    public JobNature JobNature { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool IsBiharSpecific { get; set; }
    public bool IsItSpecific { get; set; }
    public bool IsGraduateLevel { get; set; }

    // Notification & Dates (Fields 14-22, confirmed vs expected)
    public int? CycleYear { get; set; }
    public string? NotificationNumber { get; set; }
    public ExamStatus Status { get; set; }
    public string? StatusReason { get; set; }
    public DateFieldDto NotificationPublicationDate { get; set; } = new();
    public DateFieldDto ApplicationOpeningDate { get; set; } = new();
    public DateFieldDto ApplicationClosingDate { get; set; } = new();
    public DateOnly? CorrectionWindowStart { get; set; }
    public DateOnly? CorrectionWindowEnd { get; set; }
    public DateFieldDto AdmitCardDate { get; set; } = new();
    public DateFieldDto PreliminaryExamDate { get; set; } = new();
    public DateFieldDto MainsExamDate { get; set; } = new();
    public DateFieldDto InterviewDate { get; set; } = new();
    public DateFieldDto ResultDate { get; set; } = new();

    // Eligibility Summary (Fields 7-13)
    public EligibilityRuleDetailDto? EligibilityRule { get; set; }
    public IReadOnlyList<EducationalQualificationDto> EducationalQualifications { get; set; } = Array.Empty<EducationalQualificationDto>();
    public IReadOnlyList<AgeRelaxationDto> AgeRelaxations { get; set; } = Array.Empty<AgeRelaxationDto>();

    // Vacancies (Fields 23-25)
    public ExamVacancyDetailDto? Vacancy { get; set; }

    // Fee, Salary, Selection, Pattern (Fields 26-31)
    public string ApplicationFee { get; set; } = "Not Announced";
    public string SalaryPayLevel { get; set; } = "Not Announced";
    public string SelectionProcedure { get; set; } = "Not Announced";
    public ExamMode ExamMode { get; set; }
    public string NegativeMarking { get; set; } = "Not Announced";
    public IReadOnlyList<ExamPatternDto> ExamPatterns { get; set; } = Array.Empty<ExamPatternDto>();

    // Syllabus, History, Cutoffs, Papers (Fields 32-34)
    public IReadOnlyList<ExamSyllabusMappingDto> Syllabi { get; set; } = Array.Empty<ExamSyllabusMappingDto>();
    public IReadOnlyList<HistoricalRecruitmentDto> HistoricalRecruitments { get; set; } = Array.Empty<HistoricalRecruitmentDto>();
    public IReadOnlyList<PreviousYearCutoffDto> PreviousYearCutoffs { get; set; } = Array.Empty<PreviousYearCutoffDto>();
    public IReadOnlyList<PreviousYearPaperDto> PreviousYearPapers { get; set; } = Array.Empty<PreviousYearPaperDto>();

    // Official Sources & Traceability (Fields 35-37 + Verification rules)
    public string? OfficialNotificationPdfUrl { get; set; }
    public string? OfficialApplicationUrl { get; set; }
    public string OfficialRecruitmentWebsite { get; set; } = string.Empty;
    public OfficialSourceTraceDto OfficialSource { get; set; } = new();

    // User-specific state
    public bool IsSaved { get; set; }
    public string? UserApplicationStatus { get; set; }
}

public class DateFieldDto
{
    public DateOnly? ConfirmedDate { get; set; }
    public DateOnly? ExpectedDate { get; set; }
    public bool IsExpected { get; set; }
    public string DisplayText { get; set; } = "Not Announced";
    public string DateLabelType { get; set; } = "Not Announced"; // Confirmed, Expected, Not Announced
}

public class EligibilityRuleDetailDto
{
    public int MinAgeYears { get; set; }
    public int? MaxAgeYears { get; set; }
    public DateOnly? AgeCutoffDate { get; set; }
    public string AgeCutoffBasisDescription { get; set; } = string.Empty;
    public string MinimumQualificationLevel { get; set; } = string.Empty;
    public bool RequiresEngineeringOrTechDegree { get; set; }
    public bool AcceptsAnyGraduateDegree { get; set; }
    public string? AcceptedEngineeringBranches { get; set; }
    public string QualificationSummary { get; set; } = string.Empty;
    public decimal? MinPercentageGeneral { get; set; }
    public decimal? MinPercentageReserved { get; set; }
    public decimal? MinCgpa { get; set; }
    public bool FinalYearAppearingEligible { get; set; }
    public bool RequiresWorkExperience { get; set; }
    public int MinWorkExperienceMonths { get; set; }
    public string? ExperienceRequirementNotes { get; set; }
    public string NationalityRequirement { get; set; } = string.Empty;
    public string? DomicileRequirement { get; set; }
    public bool BiharDomicileRequiredForReservation { get; set; }
    public string? BiharCertificateConditions { get; set; }
    public string? GenderRequirement { get; set; }
    public bool RequiresValidGateScore { get; set; }
    public string? AcceptedGatePapers { get; set; }
    public int? MinGateScore { get; set; }
    public string? AdditionalConditions { get; set; }
    public string? VerificationNotes { get; set; }
    public string? SourceReference { get; set; }
}

public class EducationalQualificationDto
{
    public string DegreeLevel { get; set; } = string.Empty;
    public string DegreeName { get; set; } = string.Empty;
    public string? AcceptedBranches { get; set; }
    public decimal? MinimumPercentage { get; set; }
    public bool IsMandatory { get; set; }
    public string? Notes { get; set; }
}

public class AgeRelaxationDto
{
    public CategoryType Category { get; set; }
    public string? SubCategory { get; set; }
    public int RelaxationYears { get; set; }
    public int? MaxUpperAgeOverride { get; set; }
    public bool RequiresDomicileForStateQuota { get; set; }
    public bool IsSupportedByNotification { get; set; }
    public string Conditions { get; set; } = string.Empty;
}

public class ExamVacancyDetailDto
{
    public int? TotalVacancies { get; set; }
    public int? ItSpecificVacancies { get; set; }
    public int? GeneralVacancies { get; set; }
    public VacancyType VacancyType { get; set; }
    public string? Notes { get; set; }
    public string? SourceReference { get; set; }
    public IReadOnlyList<CategoryVacancyItemDto> CategoryWiseVacancies { get; set; } = Array.Empty<CategoryVacancyItemDto>();
}

public class CategoryVacancyItemDto
{
    public CategoryType Category { get; set; }
    public string? SubCategory { get; set; }
    public int? VacancyCount { get; set; }
    public string? Notes { get; set; }
}

public class ExamPatternDto
{
    public string StageName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int? QuestionsCount { get; set; }
    public decimal? MaxMarks { get; set; }
    public int? DurationMinutes { get; set; }
    public string? NegativeMarkingDetails { get; set; }
    public bool IsQualifyingOnly { get; set; }
}

public class ExamSyllabusMappingDto
{
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public SubjectGroup SubjectGroup { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string? WeightageNotes { get; set; }
    public string? ExamSpecificTopicsNotes { get; set; }
    public string? OfficialSyllabusReferenceUrl { get; set; }
    public IReadOnlyList<string> Topics { get; set; } = Array.Empty<string>();
}

public class HistoricalRecruitmentDto
{
    public int CycleYear { get; set; }
    public HistoricalDataStatus DataStatus { get; set; }
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
    public IReadOnlyList<CategoryVacancyItemDto> CategoryVacancies { get; set; } = Array.Empty<CategoryVacancyItemDto>();
}

public class PreviousYearCutoffDto
{
    public int CycleYear { get; set; }
    public string StageName { get; set; } = string.Empty;
    public CategoryType Category { get; set; }
    public decimal? CutoffMarks { get; set; }
    public decimal? TotalMaxMarks { get; set; }
    public string? SourceUrl { get; set; }
    public string? Notes { get; set; }
}

public class PreviousYearPaperDto
{
    public int CycleYear { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PaperUrl { get; set; } = string.Empty;
    public string SourceLabel { get; set; } = string.Empty;
    public bool IsOfficial { get; set; }
}

public class OfficialSourceTraceDto
{
    public string SourceOrganization { get; set; } = string.Empty;
    public string OfficialSourceUrl { get; set; } = string.Empty;
    public string? NotificationPdfUrl { get; set; }
    public DateTime? DateLastCheckedUtc { get; set; }
    public DateTime? DateLastUpdatedUtc { get; set; }
    public DateOnly? SourcePublicationDate { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DataConfidence DataConfidence { get; set; }
    public string? LastChangeSummary { get; set; }
}

public class CategorySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ExamCount { get; set; }
}
