using System.ComponentModel.DataAnnotations;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.DTOs.Admin;

public class AdminCreateExamRequest
{
    [Required] public Guid CategoryId { get; set; }
    [Required] public Guid OrganizationId { get; set; }
    [Required] public Guid OfficialSourceId { get; set; }
    [Required, MaxLength(250)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string ShortName { get; set; } = string.Empty;
    [Required, MaxLength(150)] public string Slug { get; set; } = string.Empty;
    [Required, MaxLength(60)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string Department { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string JobDesignation { get; set; } = string.Empty;
    public GroupClassification GroupClassification { get; set; }
    public JobNature JobNature { get; set; }
    [Required, MaxLength(500)] public string OfficialRecruitmentWebsite { get; set; } = string.Empty;
    public bool IsBiharSpecific { get; set; }
    public bool IsItSpecific { get; set; }
    public bool IsGraduateLevel { get; set; } = true;
    public string Summary { get; set; } = string.Empty;
    public string? DefaultSalaryPayLevel { get; set; }
    public string? DefaultSelectionProcedure { get; set; }
    public ExamMode DefaultExamMode { get; set; } = ExamMode.ComputerBasedTest;
    public string? DefaultNegativeMarking { get; set; }
    public bool RequireApprovalBeforePublish { get; set; }
}

public class AdminUpdateExamRequest
{
    [Required, MaxLength(250)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string ShortName { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string Department { get; set; } = string.Empty;
    [Required, MaxLength(250)] public string JobDesignation { get; set; } = string.Empty;
    public GroupClassification GroupClassification { get; set; }
    public JobNature JobNature { get; set; }
    [Required, MaxLength(500)] public string OfficialRecruitmentWebsite { get; set; } = string.Empty;
    public bool IsBiharSpecific { get; set; }
    public bool IsItSpecific { get; set; }
    public bool IsGraduateLevel { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string? DefaultSalaryPayLevel { get; set; }
    public string? DefaultSelectionProcedure { get; set; }
    public ExamMode DefaultExamMode { get; set; }
    public string? DefaultNegativeMarking { get; set; }
}

public class AdminUpdateNotificationDatesRequest
{
    public int CycleYear { get; set; }
    public string? NotificationNumber { get; set; }
    public string? Title { get; set; }
    public ExamStatus Status { get; set; }
    public string? StatusReason { get; set; }
    public DateOnly? NotificationDate { get; set; }
    public DateOnly? ExpectedNotificationDate { get; set; }
    public bool IsNotificationDateExpected { get; set; }
    public DateOnly? ApplicationStartDate { get; set; }
    public DateOnly? ExpectedApplicationStartDate { get; set; }
    public bool IsApplicationStartExpected { get; set; }
    public DateOnly? ApplicationEndDate { get; set; }
    public DateOnly? ExpectedApplicationEndDate { get; set; }
    public bool IsApplicationEndExpected { get; set; }
    public DateOnly? CorrectionWindowStart { get; set; }
    public DateOnly? CorrectionWindowEnd { get; set; }
    public DateOnly? AdmitCardDate { get; set; }
    public DateOnly? ExpectedAdmitCardDate { get; set; }
    public bool IsAdmitCardExpected { get; set; }
    public DateOnly? ExamDate { get; set; }
    public DateOnly? ExpectedExamDate { get; set; }
    public bool IsExamDateExpected { get; set; }
    public DateOnly? PrelimExamDate { get; set; }
    public DateOnly? ExpectedPrelimExamDate { get; set; }
    public DateOnly? MainsExamDate { get; set; }
    public DateOnly? ExpectedMainsExamDate { get; set; }
    public DateOnly? InterviewDate { get; set; }
    public DateOnly? ResultDate { get; set; }
    public DateOnly? AgeCutoffDate { get; set; }
    public string? OfficialNotificationPdfUrl { get; set; }
    public string? OfficialApplicationUrl { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.VerifiedOfficial;
    public DataConfidence DataConfidence { get; set; } = DataConfidence.ConfirmedOfficial;
    public string? ChangeSummary { get; set; }
}

public class AdminUpdateVacanciesRequest
{
    public int? TotalVacancies { get; set; }
    public int? ItSpecificVacancies { get; set; }
    public int? GeneralVacancies { get; set; }
    public VacancyType VacancyType { get; set; }
    public string? Notes { get; set; }
    public string? SourceReference { get; set; }
    public List<AdminCategoryVacancyInput> CategoryWiseVacancies { get; set; } = new();
}

public class AdminCategoryVacancyInput
{
    public CategoryType Category { get; set; }
    public string? SubCategory { get; set; }
    public int? VacancyCount { get; set; }
    public string? Notes { get; set; }
}

public class AdminAddHistoricalRequest
{
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
}

public class AdminAddSyllabusMappingRequest
{
    [Required] public Guid SyllabusSubjectId { get; set; }
    [Required, MaxLength(120)] public string StageName { get; set; } = "Preliminary & Mains";
    public string? WeightageNotes { get; set; }
    public string? ExamSpecificTopicsNotes { get; set; }
    public string? OfficialSyllabusReferenceUrl { get; set; }
}

public class AdminChangeStatusRequest
{
    public ExamStatus Status { get; set; }
    [Required, MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class AdminApproveRecordRequest
{
    public bool Approve { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.VerifiedOfficial;
    public DataConfidence DataConfidence { get; set; } = DataConfidence.ConfirmedOfficial;
    public string? Notes { get; set; }
}

public class AdminOfficialSourceDto
{
    public Guid Id { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public string OfficialSourceUrl { get; set; } = string.Empty;
    public string? CalendarOrFeedUrl { get; set; }
    public DateTime? DateLastCheckedUtc { get; set; }
    public DateTime? DateLastUpdatedUtc { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DataConfidence DataConfidence { get; set; }
    public int? LastHttpStatusCode { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

public class AdminSourceVerificationResultDto
{
    public Guid OfficialSourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; }
    public bool ChangesDetected { get; set; }
    public VerificationOutcome Outcome { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CheckedAtUtc { get; set; }
}

public class AdminAuditLogDto
{
    public Guid Id { get; set; }
    public string? UserEmail { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? Reason { get; set; }
    public DateTime TimestampUtc { get; set; }
}

public class AdminUserSummaryDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public string? DomicileState { get; set; }
    public string? GraduationQualification { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class AdminPendingItemDto
{
    public Guid ExamId { get; set; }
    public Guid? ExamNotificationId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public bool IsExamPendingPublish { get; set; }
    public bool IsNotificationPendingApproval { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DataConfidence DataConfidence { get; set; }
    public string? LastChangeSummary { get; set; }
    public DateTime? LastChangedAtUtc { get; set; }
}

public class AdminDashboardOverviewDto
{
    public int TotalExams { get; set; }
    public int PendingApprovalsCount { get; set; }
    public int UnverifiedSourcesCount { get; set; }
    public int RecentChangesCount { get; set; }
    public int TotalUsers { get; set; }
    public IReadOnlyList<AdminPendingItemDto> PendingItems { get; set; } = Array.Empty<AdminPendingItemDto>();
    public IReadOnlyList<AdminOfficialSourceDto> Sources { get; set; } = Array.Empty<AdminOfficialSourceDto>();
    public IReadOnlyList<AdminAuditLogDto> RecentAuditLogs { get; set; } = Array.Empty<AdminAuditLogDto>();
}
