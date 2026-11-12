using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Dashboard;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _db;
    private readonly IEligibilityEngine _eligibilityEngine;
    private readonly IDateTimeProvider _dateTime;

    public DashboardService(
        IApplicationDbContext db,
        IEligibilityEngine eligibilityEngine,
        IDateTimeProvider dateTime)
    {
        _db = db;
        _eligibilityEngine = eligibilityEngine;
        _dateTime = dateTime;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid? userId, CancellationToken cancellationToken = default)
    {
        var today = _dateTime.TodayIst;
        var exams = await _db.Exams
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsApprovedForPublish)
            .Include(e => e.Category)
            .Include(e => e.Organization)
            .Include(e => e.Notifications)
                .ThenInclude(n => n.Vacancy)
            .Include(e => e.EligibilityRules)
            .Include(e => e.AgeRelaxations)
            .ToListAsync(cancellationToken);

        UserProfile? profile = null;
        var savedCount = 0;
        decimal prepProgress = 0m;

        if (userId.HasValue)
        {
            profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId.Value, cancellationToken);
            savedCount = await _db.SavedExams.CountAsync(s => s.UserId == userId.Value, cancellationToken);

            var totalTopics = await _db.SyllabusTopics.CountAsync(cancellationToken);
            if (totalTopics > 0)
            {
                var completed = await _db.UserStudyProgresses
                    .CountAsync(p => p.UserId == userId.Value && p.Status == StudyProgressStatus.Completed, cancellationToken);
                prepProgress = Math.Round(completed * 100m / totalTopics, 1);
            }
        }

        var urgentDeadlines = new List<UrgentDeadlineDto>();
        var openApps = new List<DashboardExamHighlightDto>();
        var upcomingExamDates = new List<DashboardExamHighlightDto>();
        var statusCounts = new Dictionary<ExamStatus, int>();

        var openCount = 0;
        var upcomingNotifCount = 0;
        var closing7 = 0;
        var closing30 = 0;
        var upcomingExamsCount = 0;
        var matchingCount = 0;
        var newlyReleasedCount = 0;
        var totalVacancies = 0;

        foreach (var exam in exams)
        {
            var current = exam.Notifications.FirstOrDefault(n => n.IsCurrentCycle)
                ?? exam.Notifications.OrderByDescending(n => n.CycleYear).FirstOrDefault();

            var status = current?.Status ?? ExamStatus.UpcomingNotification;
            statusCounts[status] = statusCounts.GetValueOrDefault(status) + 1;

            EligibilityResultStatus? eligStatus = null;
            if (profile is not null)
            {
                eligStatus = _eligibilityEngine.EvaluateExam(exam, current, profile, today).Status;
                if (eligStatus is EligibilityResultStatus.Eligible or EligibilityResultStatus.PotentiallyEligibleVerificationRequired)
                    matchingCount++;
            }

            if (status is ExamStatus.ApplicationsOpen or ExamStatus.ClosingSoon)
            {
                openCount++;
                openApps.Add(new DashboardExamHighlightDto
                {
                    ExamId = exam.Id,
                    Name = exam.Name,
                    Slug = exam.Slug,
                    CategoryName = exam.Category.Name,
                    OrganizationCode = exam.Organization.ShortCode,
                    Status = status,
                    RelevantDate = current?.ApplicationEndDate ?? current?.ExpectedApplicationEndDate,
                    IsExpected = current?.IsApplicationEndExpected ?? false,
                    TotalVacancies = current?.Vacancy?.TotalVacancies,
                    EligibilityStatus = eligStatus
                });
            }

            if (status == ExamStatus.UpcomingNotification)
                upcomingNotifCount++;

            if (status == ExamStatus.NotificationReleased ||
                (current?.NotificationDate.HasValue == true && current.NotificationDate.Value >= today.AddDays(-15)))
                newlyReleasedCount++;

            var endDate = current?.ApplicationEndDate;
            if (endDate.HasValue && endDate.Value >= today && status is ExamStatus.ApplicationsOpen or ExamStatus.ClosingSoon or ExamStatus.NotificationReleased)
            {
                var days = endDate.Value.DayNumber - today.DayNumber;
                if (days <= 7) closing7++;
                if (days <= 30)
                {
                    closing30++;
                    urgentDeadlines.Add(new UrgentDeadlineDto
                    {
                        ExamId = exam.Id,
                        ExamName = exam.Name,
                        Slug = exam.Slug,
                        OrganizationCode = exam.Organization.ShortCode,
                        ApplicationEndDate = endDate.Value,
                        DaysRemaining = days,
                        UrgencyLevel = days <= 3 ? "Critical" : days <= 7 ? "High" : "Moderate",
                        OfficialApplicationUrl = current?.OfficialApplicationUrl,
                        EligibilityStatus = eligStatus
                    });
                }
            }

            var examDate = current?.ExamDate ?? current?.PrelimExamDate ?? current?.ExpectedExamDate ?? current?.ExpectedPrelimExamDate;
            if (examDate.HasValue && examDate.Value >= today)
            {
                upcomingExamsCount++;
                upcomingExamDates.Add(new DashboardExamHighlightDto
                {
                    ExamId = exam.Id,
                    Name = exam.Name,
                    Slug = exam.Slug,
                    CategoryName = exam.Category.Name,
                    OrganizationCode = exam.Organization.ShortCode,
                    Status = status,
                    RelevantDate = examDate,
                    IsExpected = (current?.ExamDate is null && current?.PrelimExamDate is null),
                    TotalVacancies = current?.Vacancy?.TotalVacancies,
                    EligibilityStatus = eligStatus
                });
            }

            if (current?.Vacancy?.VacancyType == VacancyType.Official && current.Vacancy.TotalVacancies.HasValue)
                totalVacancies += current.Vacancy.TotalVacancies.Value;
        }

        var byCategory = exams
            .GroupBy(e => new { e.Category.Name, e.Category.Slug, e.Category.DisplayOrder })
            .OrderBy(g => g.Key.DisplayOrder)
            .Select(g => new CategoryCountPointDto
            {
                CategoryName = g.Key.Name,
                CategorySlug = g.Key.Slug,
                ExamCount = g.Count(),
                OpenCount = g.Count(e =>
                {
                    var cur = e.Notifications.FirstOrDefault(n => n.IsCurrentCycle) ?? e.Notifications.FirstOrDefault();
                    return cur?.Status is ExamStatus.ApplicationsOpen or ExamStatus.ClosingSoon;
                })
            })
            .ToList();

        return new DashboardSummaryDto
        {
            TotalExamsTracked = exams.Count,
            CurrentlyOpenApplications = openCount,
            UpcomingNotificationsCount = upcomingNotifCount,
            ApplicationsClosingIn7Days = closing7,
            ApplicationsClosingIn30Days = closing30,
            UpcomingExamDatesCount = upcomingExamsCount,
            ExamsMatchingMyQualification = matchingCount,
            SavedExamsCount = savedCount,
            NewlyReleasedNotificationsCount = newlyReleasedCount,
            TotalVacanciesAnnounced = totalVacancies,
            MyPreparationProgressPercent = prepProgress,
            UrgentDeadlines = urgentDeadlines.OrderBy(u => u.DaysRemaining).Take(10).ToList(),
            OpenApplicationsList = openApps.OrderBy(o => o.RelevantDate ?? DateOnly.MaxValue).Take(8).ToList(),
            UpcomingExamDatesList = upcomingExamDates.OrderBy(u => u.RelevantDate ?? DateOnly.MaxValue).Take(8).ToList(),
            ExamsByCategory = byCategory,
            StatusDistribution = statusCounts.Select(kv => new StatusBreakdownPointDto
            {
                StatusName = kv.Key.ToString(),
                Count = kv.Value
            }).ToList()
        };
    }
}
