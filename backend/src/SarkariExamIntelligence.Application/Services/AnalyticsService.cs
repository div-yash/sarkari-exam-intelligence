using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Analytics;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IApplicationDbContext _db;

    public AnalyticsService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<VacancyAnalyticsDto> GetVacancyAnalyticsAsync(string? categorySlug, CancellationToken cancellationToken = default)
    {
        var examQuery = _db.Exams
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsApprovedForPublish)
            .Include(e => e.Category)
            .Include(e => e.HistoricalRecruitments)
                .ThenInclude(h => h.CategoryVacancies)
            .Include(e => e.Notifications)
                .ThenInclude(n => n.Vacancy!)
                    .ThenInclude(v => v.CategoryWiseVacancies)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(categorySlug))
            examQuery = examQuery.Where(e => e.Category.Slug == categorySlug);

        var exams = await examQuery.ToListAsync(cancellationToken);

        // Yearly aggregation strictly from verified historical rows with non-null vacancies
        var byYear = exams
            .SelectMany(e => e.HistoricalRecruitments.Select(h => new { Exam = e, History = h }))
            .Where(x => x.History.DataStatus == HistoricalDataStatus.Verified && x.History.TotalVacancies.HasValue)
            .GroupBy(x => x.History.CycleYear)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var total = g.Sum(x => x.History.TotalVacancies ?? 0);
                var itSum = g.Sum(x => x.History.ItVacancies ?? (x.Exam.IsItSpecific ? x.History.TotalVacancies ?? 0 : 0));
                return new YearlyVacancyPointDto
                {
                    Year = g.Key,
                    TotalAnnouncedVacancies = total,
                    ItSpecificVacancies = itSum,
                    GeneralVacancies = Math.Max(0, total - itSum),
                    CyclesWithVerifiedData = g.Count()
                };
            })
            .ToList();

        // By Department / Category (latest verified or active official vacancies)
        var byDept = exams
            .GroupBy(e => e.Category.Name)
            .Select(g =>
            {
                var total = 0;
                var itTotal = 0;
                foreach (var ex in g)
                {
                    var latestOfficial = ex.Notifications
                        .FirstOrDefault(n => n.IsCurrentCycle && n.Vacancy?.VacancyType == VacancyType.Official && n.Vacancy.TotalVacancies.HasValue)
                        ?.Vacancy;

                    var val = latestOfficial?.TotalVacancies
                        ?? ex.HistoricalRecruitments
                            .Where(h => h.DataStatus == HistoricalDataStatus.Verified && h.TotalVacancies.HasValue)
                            .OrderByDescending(h => h.CycleYear)
                            .FirstOrDefault()?.TotalVacancies
                        ?? 0;

                    var itVal = latestOfficial?.ItSpecificVacancies
                        ?? (ex.IsItSpecific ? val : 0);

                    total += val;
                    itTotal += itVal;
                }

                return new DepartmentVacancyDto
                {
                    CategoryName = g.Key,
                    TotalVacancies = total,
                    ItSpecificVacancies = itTotal,
                    ExamCount = g.Count()
                };
            })
            .OrderByDescending(d => d.TotalVacancies)
            .ToList();

        // By Exam summary
        var byExam = exams
            .Select(e =>
            {
                var verifiedRows = e.HistoricalRecruitments
                    .Where(h => h.DataStatus == HistoricalDataStatus.Verified && h.TotalVacancies.HasValue)
                    .OrderByDescending(h => h.CycleYear)
                    .ToList();

                var latestRow = e.HistoricalRecruitments.OrderByDescending(h => h.CycleYear).FirstOrDefault();
                var latestOfficialVac = e.Notifications
                    .FirstOrDefault(n => n.IsCurrentCycle && n.Vacancy?.VacancyType == VacancyType.Official)
                    ?.Vacancy;

                return new ExamVacancySummaryDto
                {
                    ExamId = e.Id,
                    ExamName = e.ShortName,
                    ExamSlug = e.Slug,
                    CategoryName = e.Category.Name,
                    LatestCycleYear = latestRow?.CycleYear ?? 2026,
                    LatestVacancies = latestOfficialVac?.TotalVacancies ?? verifiedRows.FirstOrDefault()?.TotalVacancies,
                    FiveYearTotalVacancies = verifiedRows.Count > 0 ? verifiedRows.Sum(r => r.TotalVacancies ?? 0) : null,
                    LatestDataStatus = latestOfficialVac?.VacancyType == VacancyType.Official
                        ? "Official"
                        : latestRow?.DataStatus.ToString() ?? "NotAnnounced"
                };
            })
            .OrderByDescending(x => x.LatestVacancies ?? -1)
            .ToList();

        var totalIt = byDept.Sum(d => d.ItSpecificVacancies);
        var totalAll = byDept.Sum(d => d.TotalVacancies);

        // Category-wise breakdown aggregated from current official CategoryWiseVacancies
        var catVacancies = exams
            .SelectMany(e => e.Notifications.Where(n => n.IsCurrentCycle && n.Vacancy != null))
            .SelectMany(n => n.Vacancy!.CategoryWiseVacancies)
            .Where(cv => cv.VacancyCount.HasValue)
            .GroupBy(cv => cv.Category)
            .Select(g => new CategoryAggregateVacancyDto
            {
                CategoryName = g.Key.ToString(),
                TotalVacancies = g.Sum(x => x.VacancyCount ?? 0)
            })
            .OrderByDescending(c => c.TotalVacancies)
            .ToList();

        return new VacancyAnalyticsDto
        {
            VacanciesByYear = byYear,
            VacanciesByDepartment = byDept,
            VacanciesByExam = byExam,
            ItVsGeneralSplit = new ItVsGeneralVacancyDto
            {
                ItSpecificVacancies = totalIt,
                GeneralAndOtherVacancies = Math.Max(0, totalAll - totalIt)
            },
            CategoryWiseVacancies = catVacancies
        };
    }

    public async Task<HistoricalComparisonResponseDto> GetExamHistoricalComparisonAsync(Guid examId, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams
            .AsNoTracking()
            .Include(e => e.Organization)
            .Include(e => e.HistoricalRecruitments)
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        return new HistoricalComparisonResponseDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            OrganizationName = exam.Organization.Name,
            Cycles = exam.HistoricalRecruitments
                .OrderBy(h => h.CycleYear)
                .Select(h => new HistoricalCycleRowDto
                {
                    CycleYear = h.CycleYear,
                    DataStatus = h.DataStatus.ToString(),
                    NotificationDate = h.NotificationDate,
                    ApplicationStartDate = h.ApplicationStartDate,
                    ApplicationEndDate = h.ApplicationEndDate,
                    PrelimExamDate = h.PrelimExamDate,
                    MainsExamDate = h.MainsExamDate,
                    TotalVacancies = h.TotalVacancies,
                    ItVacancies = h.ItVacancies,
                    GeneralVacancies = h.GeneralVacancies,
                    SelectionPatternSummary = h.SelectionPatternSummary,
                    OfficialSourceUrl = h.OfficialSourceUrl,
                    Notes = h.Notes
                }).ToList()
        };
    }
}
