using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Notifications;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class NotificationOrchestratorService : INotificationOrchestratorService
{
    private readonly IApplicationDbContext _db;
    private readonly IEligibilityEngine _eligibilityEngine;
    private readonly IEmailSender _emailSender;
    private readonly IDateTimeProvider _dateTime;

    public NotificationOrchestratorService(
        IApplicationDbContext db,
        IEligibilityEngine eligibilityEngine,
        IEmailSender emailSender,
        IDateTimeProvider dateTime)
    {
        _db = db;
        _eligibilityEngine = eligibilityEngine;
        _emailSender = emailSender;
        _dateTime = dateTime;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .Include(n => n.Exam)
            .AsQueryable();

        if (unreadOnly)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.ScheduledForUtc)
            .Take(100)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                ExamId = n.ExamId,
                ExamName = n.Exam != null ? n.Exam.ShortName : null,
                ExamSlug = n.Exam != null ? n.Exam.Slug : null,
                NotificationType = n.NotificationType,
                Title = n.Title,
                Message = n.Message,
                Channel = n.Channel,
                IsRead = n.IsRead,
                ReadAtUtc = n.ReadAtUtc,
                EmailSent = n.EmailSent,
                ScheduledForUtc = n.ScheduledForUtc
            }).ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var item = await _db.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Notification not found.");

        if (!item.IsRead)
        {
            item.IsRead = true;
            item.ReadAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unread = await _db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;
        foreach (var item in unread)
        {
            item.IsRead = true;
            item.ReadAtUtc = now;
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<NotificationPreferenceDto> GetPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var pref = await _db.NotificationPreferences.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (pref is null)
        {
            return new NotificationPreferenceDto
            {
                EnableInApp = true,
                EnableEmail = true,
                AlertOnNewNotification = true,
                AlertOnApplicationOpen = true,
                Alert30DaysBefore = true,
                Alert7DaysBefore = true,
                Alert3DaysBefore = true,
                Alert1DayBefore = true,
                AlertOnExamDateChange = true,
                AlertOnAdmitCard = true,
                AlertOnResult = true
            };
        }

        return MapPref(pref);
    }

    public async Task<NotificationPreferenceDto> UpdatePreferencesAsync(Guid userId, UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken = default)
    {
        var pref = await _db.NotificationPreferences.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (pref is null)
        {
            pref = new NotificationPreference { UserId = userId };
            _db.NotificationPreferences.Add(pref);
        }

        pref.EnableInApp = request.EnableInApp;
        pref.EnableEmail = request.EnableEmail;
        pref.AlertOnNewNotification = request.AlertOnNewNotification;
        pref.AlertOnApplicationOpen = request.AlertOnApplicationOpen;
        pref.Alert30DaysBefore = request.Alert30DaysBefore;
        pref.Alert7DaysBefore = request.Alert7DaysBefore;
        pref.Alert3DaysBefore = request.Alert3DaysBefore;
        pref.Alert1DayBefore = request.Alert1DayBefore;
        pref.AlertOnExamDateChange = request.AlertOnExamDateChange;
        pref.AlertOnAdmitCard = request.AlertOnAdmitCard;
        pref.AlertOnResult = request.AlertOnResult;
        pref.OverrideEmail = string.IsNullOrWhiteSpace(request.OverrideEmail) ? null : request.OverrideEmail.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return MapPref(pref);
    }

    public async Task<int> RunScheduledNotificationChecksAsync(CancellationToken cancellationToken = default)
    {
        var today = _dateTime.TodayIst;
        var nowUtc = _dateTime.UtcNow;

        var profiles = await _db.UserProfiles.AsNoTracking().ToListAsync(cancellationToken);
        if (profiles.Count == 0) return 0;

        var prefsByUser = (await _db.NotificationPreferences.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(p => p.UserId);

        var savedByUser = (await _db.SavedExams.AsNoTracking().ToListAsync(cancellationToken))
            .GroupBy(s => s.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ExamId).ToHashSet());

        var exams = await _db.Exams
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsApprovedForPublish)
            .Include(e => e.Category)
            .Include(e => e.Organization)
            .Include(e => e.Notifications)
            .Include(e => e.EligibilityRules)
            .Include(e => e.AgeRelaxations)
            .ToListAsync(cancellationToken);

        var createdCount = 0;

        foreach (var profile in profiles)
        {
            prefsByUser.TryGetValue(profile.UserId, out var pref);
            var enableInApp = pref?.EnableInApp ?? true;
            var enableEmail = pref?.EnableEmail ?? true;
            if (!enableInApp && !enableEmail) continue;

            var existingKeys = (await _db.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == profile.UserId)
                .Select(n => n.DeduplicationKey)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            savedByUser.TryGetValue(profile.UserId, out var savedSet);
            savedSet ??= new HashSet<Guid>();

            foreach (var exam in exams)
            {
                var n = exam.Notifications.FirstOrDefault(x => x.IsCurrentCycle)
                    ?? exam.Notifications.OrderByDescending(x => x.CycleYear).FirstOrDefault();
                if (n is null) continue;

                var elig = _eligibilityEngine.EvaluateExam(exam, n, profile, today);
                var isRelevant = savedSet.Contains(exam.Id) || elig.Status != EligibilityResultStatus.NotEligible;
                if (!isRelevant) continue;

                // 1. New official notification released (within last 7 days)
                if ((pref?.AlertOnNewNotification ?? true) &&
                    n.NotificationDate.HasValue &&
                    n.NotificationDate.Value >= today.AddDays(-7) &&
                    n.NotificationDate.Value <= today)
                {
                    await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.NewOfficialNotification,
                        $"new-notif:{n.Id}:{n.CycleYear}",
                        $"New Official Notification: {exam.ShortName} ({n.CycleYear})",
                        $"{exam.Name} official notification released on {n.NotificationDate:dd MMM yyyy}. Eligibility: {elig.StatusLabel}.",
                        enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                }

                // 2. Applications open
                if ((pref?.AlertOnApplicationOpen ?? true) &&
                    n.Status is ExamStatus.ApplicationsOpen or ExamStatus.ClosingSoon &&
                    n.ApplicationStartDate.HasValue && n.ApplicationStartDate.Value <= today &&
                    (!n.ApplicationEndDate.HasValue || n.ApplicationEndDate.Value >= today))
                {
                    await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.ApplicationsOpened,
                        $"app-open:{n.Id}:{n.CycleYear}",
                        $"Applications Open: {exam.ShortName}",
                        $"Online applications are open for {exam.Name}. Closing date: {(n.ApplicationEndDate.HasValue ? n.ApplicationEndDate.Value.ToString("dd MMM yyyy") : "Not Announced")}.",
                        enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                }

                // 3-6. Deadline reminders: 30d, 7d, 3d, 1d before last date
                if (n.ApplicationEndDate.HasValue && n.ApplicationEndDate.Value >= today)
                {
                    var daysLeft = n.ApplicationEndDate.Value.DayNumber - today.DayNumber;

                    if (daysLeft is <= 30 and > 7 && (pref?.Alert30DaysBefore ?? true))
                    {
                        await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.Deadline30Days,
                            $"deadline-30d:{n.Id}:{n.ApplicationEndDate:yyyyMMdd}",
                            $"30-Day Reminder: {exam.ShortName}",
                            $"Application window for {exam.Name} closes in {daysLeft} days ({n.ApplicationEndDate:dd MMM yyyy}).",
                            enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                    }
                    else if (daysLeft is <= 7 and > 3 && (pref?.Alert7DaysBefore ?? true))
                    {
                        await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.Deadline7Days,
                            $"deadline-7d:{n.Id}:{n.ApplicationEndDate:yyyyMMdd}",
                            $"7-Day Urgent Reminder: {exam.ShortName}",
                            $"Only {daysLeft} days left to apply for {exam.Name} (Deadline: {n.ApplicationEndDate:dd MMM yyyy}).",
                            enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                    }
                    else if (daysLeft is <= 3 and > 1 && (pref?.Alert3DaysBefore ?? true))
                    {
                        await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.Deadline3Days,
                            $"deadline-3d:{n.Id}:{n.ApplicationEndDate:yyyyMMdd}",
                            $"3-Day Critical Alert: {exam.ShortName}",
                            $"CRITICAL: {exam.Name} closes in {daysLeft} days ({n.ApplicationEndDate:dd MMM yyyy}). Complete submission early to avoid portal congestion.",
                            enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                    }
                    else if (daysLeft <= 1 && (pref?.Alert1DayBefore ?? true))
                    {
                        await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.Deadline1Day,
                            $"deadline-1d:{n.Id}:{n.ApplicationEndDate:yyyyMMdd}",
                            daysLeft == 0 ? $"FINAL DAY TODAY: {exam.ShortName}" : $"1-Day Final Alert: {exam.ShortName}",
                            $"FINAL CALL: {exam.Name} application deadline is {n.ApplicationEndDate:dd MMM yyyy} ({(daysLeft == 0 ? "today" : "tomorrow")}).",
                            enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                    }
                }

                // 7. Exam date / notification changes
                if ((pref?.AlertOnExamDateChange ?? true) && n.LastChangedAtUtc.HasValue && n.LastChangedAtUtc.Value >= nowUtc.AddDays(-7) && !string.IsNullOrWhiteSpace(n.LastChangeSummary))
                {
                    await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.ExamDateChanged,
                        $"change:{n.Id}:{n.LastChangedAtUtc.Value:yyyyMMddHH}",
                        $"Update Detected: {exam.ShortName}",
                        $"{exam.Name} update: {n.LastChangeSummary}",
                        enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                }

                // 8. Admit card released
                if ((pref?.AlertOnAdmitCard ?? true) && n.Status == ExamStatus.AdmitCardReleased)
                {
                    await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.AdmitCardReleased,
                        $"admit-card:{n.Id}:{n.CycleYear}",
                        $"Admit Card Released: {exam.ShortName}",
                        $"Admit card / hall ticket for {exam.Name} ({n.CycleYear}) is now available on {exam.OfficialRecruitmentWebsite}.",
                        enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                }

                // 9. Result declared
                if ((pref?.AlertOnResult ?? true) && n.Status == ExamStatus.ResultDeclared)
                {
                    await TryCreateAlertAsync(profile.UserId, exam, n, NotificationType.ResultDeclared,
                        $"result:{n.Id}:{n.CycleYear}",
                        $"Result Declared: {exam.ShortName}",
                        $"Official result declared for {exam.Name} ({n.CycleYear}). Check the official portal.",
                        enableInApp, enableEmail, pref?.OverrideEmail, existingKeys, nowUtc, cancellationToken);
                }
            }
        }

        if (createdCount > 0)
            await _db.SaveChangesAsync(cancellationToken);

        return createdCount;

        async Task TryCreateAlertAsync(
            Guid uid, Exam exam, ExamNotification notif, NotificationType type, string dedupKey,
            string title, string message, bool inApp, bool email, string? overrideEmail,
            HashSet<string> keys, DateTime now, CancellationToken ct)
        {
            if (keys.Contains(dedupKey)) return;
            keys.Add(dedupKey);

            var channel = inApp && email ? NotificationChannel.Both : inApp ? NotificationChannel.InApp : NotificationChannel.Email;
            var alert = new Notification
            {
                UserId = uid,
                ExamId = exam.Id,
                ExamNotificationId = notif.Id,
                NotificationType = type,
                Title = title,
                Message = message,
                DeduplicationKey = dedupKey,
                Channel = channel,
                IsRead = false,
                ScheduledForUtc = now
            };

            if (email && !string.IsNullOrWhiteSpace(overrideEmail))
            {
                try
                {
                    await _emailSender.SendEmailAsync(overrideEmail, title, $"<p>{message}</p>", ct);
                    alert.EmailSent = true;
                    alert.EmailSentAtUtc = DateTime.UtcNow;
                }
                catch
                {
                    alert.EmailSent = false;
                }
            }

            _db.Notifications.Add(alert);
            createdCount++;
        }
    }

    private static NotificationPreferenceDto MapPref(NotificationPreference p) => new()
    {
        EnableInApp = p.EnableInApp,
        EnableEmail = p.EnableEmail,
        AlertOnNewNotification = p.AlertOnNewNotification,
        AlertOnApplicationOpen = p.AlertOnApplicationOpen,
        Alert30DaysBefore = p.Alert30DaysBefore,
        Alert7DaysBefore = p.Alert7DaysBefore,
        Alert3DaysBefore = p.Alert3DaysBefore,
        Alert1DayBefore = p.Alert1DayBefore,
        AlertOnExamDateChange = p.AlertOnExamDateChange,
        AlertOnAdmitCard = p.AlertOnAdmitCard,
        AlertOnResult = p.AlertOnResult,
        OverrideEmail = p.OverrideEmail
    };
}
