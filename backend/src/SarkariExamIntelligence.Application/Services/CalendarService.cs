using System.Text;
using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Dashboard;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class CalendarService : ICalendarService
{
    private readonly IApplicationDbContext _db;

    public CalendarService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CalendarEventDto>> GetEventsAsync(
        CalendarFilterRequest filter,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Exams
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsApprovedForPublish)
            .Include(e => e.Category)
            .Include(e => e.Organization)
            .Include(e => e.Notifications)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.CategorySlug))
            query = query.Where(e => e.Category.Slug == filter.CategorySlug);

        HashSet<Guid>? savedIds = null;
        if (filter.OnlySavedOrEligible && userId.HasValue)
        {
            savedIds = (await _db.SavedExams
                .AsNoTracking()
                .Where(s => s.UserId == userId.Value)
                .Select(s => s.ExamId)
                .ToListAsync(cancellationToken))
                .ToHashSet();
        }

        var exams = await query.ToListAsync(cancellationToken);
        var events = new List<CalendarEventDto>();

        foreach (var exam in exams)
        {
            if (savedIds is not null && !savedIds.Contains(exam.Id))
                continue;

            var n = exam.Notifications.FirstOrDefault(x => x.IsCurrentCycle)
                ?? exam.Notifications.OrderByDescending(x => x.CycleYear).FirstOrDefault();
            if (n is null) continue;

            void AddEvent(CalendarEventType type, string title, DateOnly? confirmed, DateOnly? expected, bool isExpectedFlag)
            {
                var dt = confirmed ?? expected;
                if (!dt.HasValue) return;
                var isExp = !confirmed.HasValue && (isExpectedFlag || expected.HasValue);
                if (filter.OnlyConfirmed && isExp) return;
                if (filter.FromDate.HasValue && dt.Value < filter.FromDate.Value) return;
                if (filter.ToDate.HasValue && dt.Value > filter.ToDate.Value) return;

                events.Add(new CalendarEventDto
                {
                    ExamId = exam.Id,
                    ExamName = exam.ShortName,
                    ExamSlug = exam.Slug,
                    CategoryName = exam.Category.Name,
                    CategorySlug = exam.Category.Slug,
                    OrganizationCode = exam.Organization.ShortCode,
                    EventType = type,
                    Title = $"{exam.ShortName} — {title}" + (isExp ? " (Expected)" : ""),
                    EventDate = dt.Value,
                    IsExpected = isExp,
                    ExamStatus = n.Status
                });
            }

            AddEvent(CalendarEventType.NotificationRelease, "Notification Release", n.NotificationDate, n.ExpectedNotificationDate, n.IsNotificationDateExpected);
            AddEvent(CalendarEventType.ApplicationOpen, "Applications Open", n.ApplicationStartDate, n.ExpectedApplicationStartDate, n.IsApplicationStartExpected);
            AddEvent(CalendarEventType.ApplicationDeadline, "Application Deadline", n.ApplicationEndDate, n.ExpectedApplicationEndDate, n.IsApplicationEndExpected);
            AddEvent(CalendarEventType.AdmitCardRelease, "Admit Card Release", n.AdmitCardDate, n.ExpectedAdmitCardDate, n.IsAdmitCardExpected);
            AddEvent(CalendarEventType.PrelimsExam, n.MainsExamDate.HasValue || n.ExpectedMainsExamDate.HasValue ? "Prelims / Tier-I Exam" : "Examination Date",
                n.PrelimExamDate ?? n.ExamDate, n.ExpectedPrelimExamDate ?? n.ExpectedExamDate, n.IsPrelimDateExpected || n.IsExamDateExpected);
            AddEvent(CalendarEventType.MainsExam, "Mains / Tier-II Exam", n.MainsExamDate, n.ExpectedMainsExamDate, n.IsMainsDateExpected);
            AddEvent(CalendarEventType.ResultDeclaration, "Result Declaration", n.ResultDate, n.ExpectedResultDate, n.IsResultDateExpected);
        }

        return events.OrderBy(e => e.EventDate).ThenBy(e => e.ExamName).ToList();
    }

    public async Task<string> GenerateIcsExportAsync(
        CalendarFilterRequest filter,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var events = await GetEventsAsync(filter, userId, cancellationToken);
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//SarkariExamIntelligence//ExamCalendar//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");

        foreach (var ev in events)
        {
            var dateStr = ev.EventDate.ToString("yyyyMMdd");
            var nextDayStr = ev.EventDate.AddDays(1).ToString("yyyyMMdd");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{ev.ExamId}-{ev.EventType}-{dateStr}@sarkariexamintelligence.in");
            sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"DTSTART;VALUE=DATE:{dateStr}");
            sb.AppendLine($"DTEND;VALUE=DATE:{nextDayStr}");
            sb.AppendLine($"SUMMARY:{EscapeIcs(ev.Title)}");
            sb.AppendLine($"DESCRIPTION:Category: {EscapeIcs(ev.CategoryName)} | Org: {EscapeIcs(ev.OrganizationCode)} | Status: {ev.ExamStatus}");
            sb.AppendLine("END:VEVENT");
        }

        sb.AppendLine("END:VCALENDAR");
        return sb.ToString();
    }

    private static string EscapeIcs(string input) =>
        input.Replace("\\", "\\\\").Replace(",", "\\,").Replace(";", "\\;").Replace("\n", "\\n");
}
