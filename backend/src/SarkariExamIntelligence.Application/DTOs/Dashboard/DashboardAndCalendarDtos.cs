using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    // All 11 required widgets
    public int TotalExamsTracked { get; set; }
    public int CurrentlyOpenApplications { get; set; }
    public int UpcomingNotificationsCount { get; set; }
    public int ApplicationsClosingIn7Days { get; set; }
    public int ApplicationsClosingIn30Days { get; set; }
    public int UpcomingExamDatesCount { get; set; }
    public int ExamsMatchingMyQualification { get; set; }
    public int SavedExamsCount { get; set; }
    public int NewlyReleasedNotificationsCount { get; set; }
    public int TotalVacanciesAnnounced { get; set; }
    public decimal MyPreparationProgressPercent { get; set; }

    public IReadOnlyList<UrgentDeadlineDto> UrgentDeadlines { get; set; } = Array.Empty<UrgentDeadlineDto>();
    public IReadOnlyList<DashboardExamHighlightDto> OpenApplicationsList { get; set; } = Array.Empty<DashboardExamHighlightDto>();
    public IReadOnlyList<DashboardExamHighlightDto> UpcomingExamDatesList { get; set; } = Array.Empty<DashboardExamHighlightDto>();
    public IReadOnlyList<CategoryCountPointDto> ExamsByCategory { get; set; } = Array.Empty<CategoryCountPointDto>();
    public IReadOnlyList<StatusBreakdownPointDto> StatusDistribution { get; set; } = Array.Empty<StatusBreakdownPointDto>();
}

public class UrgentDeadlineDto
{
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public DateOnly ApplicationEndDate { get; set; }
    public int DaysRemaining { get; set; }
    public string UrgencyLevel { get; set; } = "Urgent"; // Critical (<=3d), High (<=7d), Moderate (<=30d)
    public string? OfficialApplicationUrl { get; set; }
    public EligibilityResultStatus? EligibilityStatus { get; set; }
}

public class DashboardExamHighlightDto
{
    public Guid ExamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public ExamStatus Status { get; set; }
    public DateOnly? RelevantDate { get; set; }
    public bool IsExpected { get; set; }
    public int? TotalVacancies { get; set; }
    public EligibilityResultStatus? EligibilityStatus { get; set; }
}

public class CategoryCountPointDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public int ExamCount { get; set; }
    public int OpenCount { get; set; }
}

public class StatusBreakdownPointDto
{
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class CalendarFilterRequest
{
    public string? CategorySlug { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public bool OnlyConfirmed { get; set; }
    public bool OnlySavedOrEligible { get; set; }
}

public class CalendarEventDto
{
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamSlug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public CalendarEventType EventType { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly EventDate { get; set; }
    public bool IsExpected { get; set; }
    public ExamStatus ExamStatus { get; set; }
}
