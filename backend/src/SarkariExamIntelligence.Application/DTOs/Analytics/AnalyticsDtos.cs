namespace SarkariExamIntelligence.Application.DTOs.Analytics;

public class VacancyAnalyticsDto
{
    public string Disclaimer { get; set; } =
        "Historical vacancy counts reflect officially announced figures from past recruitment cycles. " +
        "Cycles awaiting official notification are marked explicitly and historical trends are NOT predictions of future vacancies.";

    public IReadOnlyList<YearlyVacancyPointDto> VacanciesByYear { get; set; } = Array.Empty<YearlyVacancyPointDto>();
    public IReadOnlyList<DepartmentVacancyDto> VacanciesByDepartment { get; set; } = Array.Empty<DepartmentVacancyDto>();
    public IReadOnlyList<ExamVacancySummaryDto> VacanciesByExam { get; set; } = Array.Empty<ExamVacancySummaryDto>();
    public ItVsGeneralVacancyDto ItVsGeneralSplit { get; set; } = new();
    public IReadOnlyList<CategoryAggregateVacancyDto> CategoryWiseVacancies { get; set; } = Array.Empty<CategoryAggregateVacancyDto>();
}

public class YearlyVacancyPointDto
{
    public int Year { get; set; }
    public int TotalAnnouncedVacancies { get; set; }
    public int ItSpecificVacancies { get; set; }
    public int GeneralVacancies { get; set; }
    public int CyclesWithVerifiedData { get; set; }
}

public class DepartmentVacancyDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalVacancies { get; set; }
    public int ItSpecificVacancies { get; set; }
    public int ExamCount { get; set; }
}

public class ExamVacancySummaryDto
{
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamSlug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int LatestCycleYear { get; set; }
    public int? LatestVacancies { get; set; }
    public int? FiveYearTotalVacancies { get; set; }
    public string LatestDataStatus { get; set; } = string.Empty;
}

public class ItVsGeneralVacancyDto
{
    public int ItSpecificVacancies { get; set; }
    public int GeneralAndOtherVacancies { get; set; }
}

public class CategoryAggregateVacancyDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalVacancies { get; set; }
}

public class HistoricalComparisonResponseDto
{
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string Disclaimer { get; set; } = "Historical data is strictly sourced from verified cycles; unavailable years are never interpolated.";
    public IReadOnlyList<HistoricalCycleRowDto> Cycles { get; set; } = Array.Empty<HistoricalCycleRowDto>();
}

public class HistoricalCycleRowDto
{
    public int CycleYear { get; set; }
    public string DataStatus { get; set; } = string.Empty;
    public DateOnly? NotificationDate { get; set; }
    public DateOnly? ApplicationStartDate { get; set; }
    public DateOnly? ApplicationEndDate { get; set; }
    public DateOnly? PrelimExamDate { get; set; }
    public DateOnly? MainsExamDate { get; set; }
    public int? TotalVacancies { get; set; }
    public int? ItVacancies { get; set; }
    public int? GeneralVacancies { get; set; }
    public string? SelectionPatternSummary { get; set; }
    public string? OfficialSourceUrl { get; set; }
    public string? Notes { get; set; }
}
