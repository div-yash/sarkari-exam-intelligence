using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Admin;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class AdminService : IAdminService
{
    private readonly IApplicationDbContext _db;
    private readonly IAuditService _audit;

    public AdminService(IApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<AdminDashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        var exams = await _db.Exams
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .Include(e => e.Organization)
            .Include(e => e.Notifications)
            .ToListAsync(cancellationToken);

        var sources = await GetSourcesAsync(cancellationToken);
        var logs = await GetAuditLogsAsync(50, cancellationToken);
        var totalUsers = await _db.UserProfiles.CountAsync(cancellationToken);

        var pending = new List<AdminPendingItemDto>();
        var recentChanges = 0;

        foreach (var exam in exams)
        {
            var cur = exam.Notifications.FirstOrDefault(n => n.IsCurrentCycle) ?? exam.Notifications.FirstOrDefault();
            if (cur?.LastChangedAtUtc >= DateTime.UtcNow.AddDays(-14))
                recentChanges++;

            if (!exam.IsApprovedForPublish || cur?.IsPendingApproval == true ||
                exam.VerificationStatus is VerificationStatus.PendingApproval or VerificationStatus.NeedsReverification or VerificationStatus.ThirdPartyDiscovered)
            {
                pending.Add(new AdminPendingItemDto
                {
                    ExamId = exam.Id,
                    ExamNotificationId = cur?.Id,
                    ExamName = exam.Name,
                    OrganizationCode = exam.Organization.ShortCode,
                    IsExamPendingPublish = !exam.IsApprovedForPublish,
                    IsNotificationPendingApproval = cur?.IsPendingApproval ?? false,
                    VerificationStatus = cur?.VerificationStatus ?? exam.VerificationStatus,
                    DataConfidence = cur?.DataConfidence ?? exam.DataConfidence,
                    LastChangeSummary = cur?.LastChangeSummary,
                    LastChangedAtUtc = cur?.LastChangedAtUtc
                });
            }
        }

        return new AdminDashboardOverviewDto
        {
            TotalExams = exams.Count,
            PendingApprovalsCount = pending.Count,
            UnverifiedSourcesCount = sources.Count(s => s.VerificationStatus != VerificationStatus.VerifiedOfficial),
            RecentChangesCount = recentChanges,
            TotalUsers = totalUsers,
            PendingItems = pending,
            Sources = sources,
            RecentAuditLogs = logs
        };
    }

    public async Task<Guid> CreateExamAsync(AdminCreateExamRequest request, CancellationToken cancellationToken = default)
    {
        if (await _db.Exams.AnyAsync(e => e.Slug == request.Slug || e.Code == request.Code, cancellationToken))
            throw new ConflictException($"Exam with slug '{request.Slug}' or code '{request.Code}' already exists.");

        var exam = new Exam
        {
            CategoryId = request.CategoryId,
            OrganizationId = request.OrganizationId,
            OfficialSourceId = request.OfficialSourceId,
            Name = request.Name.Trim(),
            ShortName = request.ShortName.Trim(),
            Slug = request.Slug.Trim().ToLowerInvariant(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Department = request.Department.Trim(),
            JobDesignation = request.JobDesignation.Trim(),
            GroupClassification = request.GroupClassification,
            JobNature = request.JobNature,
            OfficialRecruitmentWebsite = request.OfficialRecruitmentWebsite.Trim(),
            IsBiharSpecific = request.IsBiharSpecific,
            IsItSpecific = request.IsItSpecific,
            IsGraduateLevel = request.IsGraduateLevel,
            Summary = request.Summary,
            DefaultSalaryPayLevel = request.DefaultSalaryPayLevel,
            DefaultSelectionProcedure = request.DefaultSelectionProcedure,
            DefaultExamMode = request.DefaultExamMode,
            DefaultNegativeMarking = request.DefaultNegativeMarking,
            IsApprovedForPublish = !request.RequireApprovalBeforePublish,
            VerificationStatus = request.RequireApprovalBeforePublish ? VerificationStatus.PendingApproval : VerificationStatus.VerifiedOfficial,
            DataConfidence = request.RequireApprovalBeforePublish ? DataConfidence.UnverifiedDraft : DataConfidence.ConfirmedOfficial
        };

        _db.Exams.Add(exam);
        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Exam", exam.Id.ToString(), "Create", null, new { exam.Name, exam.Code, exam.IsApprovedForPublish }, "Created via Admin Dashboard", cancellationToken);
        return exam.Id;
    }

    public async Task UpdateExamAsync(Guid examId, AdminUpdateExamRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams.FirstOrDefaultAsync(e => e.Id == examId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        var oldSnapshot = new { exam.Name, exam.Department, exam.JobDesignation, exam.OfficialRecruitmentWebsite };

        exam.Name = request.Name.Trim();
        exam.ShortName = request.ShortName.Trim();
        exam.Department = request.Department.Trim();
        exam.JobDesignation = request.JobDesignation.Trim();
        exam.GroupClassification = request.GroupClassification;
        exam.JobNature = request.JobNature;
        exam.OfficialRecruitmentWebsite = request.OfficialRecruitmentWebsite.Trim();
        exam.IsBiharSpecific = request.IsBiharSpecific;
        exam.IsItSpecific = request.IsItSpecific;
        exam.IsGraduateLevel = request.IsGraduateLevel;
        exam.Summary = request.Summary;
        exam.DefaultSalaryPayLevel = request.DefaultSalaryPayLevel;
        exam.DefaultSelectionProcedure = request.DefaultSelectionProcedure;
        exam.DefaultExamMode = request.DefaultExamMode;
        exam.DefaultNegativeMarking = request.DefaultNegativeMarking;

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Exam", exam.Id.ToString(), "Update", oldSnapshot, request, "Updated exam master details", cancellationToken);
    }

    public async Task UpdateNotificationDatesAsync(Guid examId, AdminUpdateNotificationDatesRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams.Include(e => e.Notifications).FirstOrDefaultAsync(e => e.Id == examId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        var notif = exam.Notifications.FirstOrDefault(n => n.CycleYear == request.CycleYear)
            ?? exam.Notifications.FirstOrDefault(n => n.IsCurrentCycle);

        object? oldValues = null;
        if (notif is null)
        {
            notif = new ExamNotification
            {
                ExamId = exam.Id,
                CycleYear = request.CycleYear,
                IsCurrentCycle = true,
                Title = request.Title ?? $"{exam.Name} ({request.CycleYear})"
            };
            _db.ExamNotifications.Add(notif);
        }
        else
        {
            oldValues = new { notif.Status, notif.NotificationDate, notif.ApplicationStartDate, notif.ApplicationEndDate, notif.ExamDate, notif.PrelimExamDate, notif.MainsExamDate, notif.ResultDate };
        }

        notif.CycleYear = request.CycleYear;
        notif.NotificationNumber = request.NotificationNumber;
        if (!string.IsNullOrWhiteSpace(request.Title)) notif.Title = request.Title.Trim();
        notif.Status = request.Status;
        notif.StatusReason = request.StatusReason;
        notif.NotificationDate = request.NotificationDate;
        notif.ExpectedNotificationDate = request.ExpectedNotificationDate;
        notif.IsNotificationDateExpected = request.IsNotificationDateExpected;
        notif.ApplicationStartDate = request.ApplicationStartDate;
        notif.ExpectedApplicationStartDate = request.ExpectedApplicationStartDate;
        notif.IsApplicationStartExpected = request.IsApplicationStartExpected;
        notif.ApplicationEndDate = request.ApplicationEndDate;
        notif.ExpectedApplicationEndDate = request.ExpectedApplicationEndDate;
        notif.IsApplicationEndExpected = request.IsApplicationEndExpected;
        notif.CorrectionWindowStart = request.CorrectionWindowStart;
        notif.CorrectionWindowEnd = request.CorrectionWindowEnd;
        notif.AdmitCardDate = request.AdmitCardDate;
        notif.ExpectedAdmitCardDate = request.ExpectedAdmitCardDate;
        notif.IsAdmitCardExpected = request.IsAdmitCardExpected;
        notif.ExamDate = request.ExamDate;
        notif.ExpectedExamDate = request.ExpectedExamDate;
        notif.IsExamDateExpected = request.IsExamDateExpected;
        notif.PrelimExamDate = request.PrelimExamDate;
        notif.ExpectedPrelimExamDate = request.ExpectedPrelimExamDate;
        notif.MainsExamDate = request.MainsExamDate;
        notif.ExpectedMainsExamDate = request.ExpectedMainsExamDate;
        notif.InterviewDate = request.InterviewDate;
        notif.ResultDate = request.ResultDate;
        notif.AgeCutoffDate = request.AgeCutoffDate;
        if (!string.IsNullOrWhiteSpace(request.OfficialNotificationPdfUrl)) notif.OfficialNotificationPdfUrl = request.OfficialNotificationPdfUrl.Trim();
        if (!string.IsNullOrWhiteSpace(request.OfficialApplicationUrl)) notif.OfficialApplicationUrl = request.OfficialApplicationUrl.Trim();
        notif.VerificationStatus = request.VerificationStatus;
        notif.DataConfidence = request.DataConfidence;
        notif.LastVerifiedAtUtc = DateTime.UtcNow;
        notif.LastChangedAtUtc = DateTime.UtcNow;
        notif.LastChangeSummary = string.IsNullOrWhiteSpace(request.ChangeSummary) ? "Notification dates updated by admin." : request.ChangeSummary.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("ExamNotification", notif.Id.ToString(), "UpdateDates", oldValues, request, notif.LastChangeSummary, cancellationToken);
    }

    public async Task UpdateVacanciesAsync(Guid examId, AdminUpdateVacanciesRequest request, CancellationToken cancellationToken = default)
    {
        var notif = await _db.ExamNotifications
            .Include(n => n.Vacancy!)
                .ThenInclude(v => v.CategoryWiseVacancies)
            .FirstOrDefaultAsync(n => n.ExamId == examId && n.IsCurrentCycle, cancellationToken)
            ?? throw new NotFoundException("Active exam notification cycle not found.");

        var oldVal = notif.Vacancy is null ? null : new { notif.Vacancy.TotalVacancies, notif.Vacancy.ItSpecificVacancies, notif.Vacancy.VacancyType };

        if (notif.Vacancy is null)
        {
            notif.Vacancy = new ExamVacancy { ExamNotificationId = notif.Id };
            _db.ExamVacancies.Add(notif.Vacancy);
        }

        notif.Vacancy.TotalVacancies = request.TotalVacancies;
        notif.Vacancy.ItSpecificVacancies = request.ItSpecificVacancies;
        notif.Vacancy.GeneralVacancies = request.GeneralVacancies;
        notif.Vacancy.VacancyType = request.VacancyType;
        notif.Vacancy.Notes = request.Notes;
        notif.Vacancy.SourceReference = request.SourceReference;

        if (request.CategoryWiseVacancies.Count > 0)
        {
            _db.CategoryWiseVacancies.RemoveRange(notif.Vacancy.CategoryWiseVacancies);
            foreach (var cv in request.CategoryWiseVacancies)
            {
                _db.CategoryWiseVacancies.Add(new CategoryWiseVacancy
                {
                    ExamVacancyId = notif.Vacancy.Id,
                    Category = cv.Category,
                    SubCategory = cv.SubCategory,
                    VacancyCount = cv.VacancyCount,
                    Notes = cv.Notes
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("ExamVacancy", notif.Vacancy.Id.ToString(), "UpdateVacancies", oldVal, request, "Updated vacancies", cancellationToken);
    }

    public async Task AddHistoricalRecruitmentAsync(Guid examId, AdminAddHistoricalRequest request, CancellationToken cancellationToken = default)
    {
        var examExists = await _db.Exams.AnyAsync(e => e.Id == examId, cancellationToken);
        if (!examExists) throw new NotFoundException("Exam not found.");

        var existing = await _db.HistoricalRecruitments
            .FirstOrDefaultAsync(h => h.ExamId == examId && h.CycleYear == request.CycleYear, cancellationToken);

        // Rule 10: Never silently overwrite verified historical recruitment data!
        if (existing is not null && existing.DataStatus == HistoricalDataStatus.Verified)
            throw new ConflictException($"Verified historical recruitment record for cycle {request.CycleYear} already exists and cannot be silently overwritten.");

        if (existing is null)
        {
            existing = new HistoricalRecruitment
            {
                ExamId = examId,
                CycleYear = request.CycleYear
            };
            _db.HistoricalRecruitments.Add(existing);
        }

        existing.DataStatus = request.DataStatus;
        existing.NotificationDate = request.NotificationDate;
        existing.ApplicationStartDate = request.ApplicationStartDate;
        existing.ApplicationEndDate = request.ApplicationEndDate;
        existing.PrelimExamDate = request.PrelimExamDate;
        existing.MainsExamDate = request.MainsExamDate;
        existing.ResultDate = request.ResultDate;
        existing.TotalVacancies = request.TotalVacancies;
        existing.ItVacancies = request.ItVacancies;
        existing.GeneralVacancies = request.GeneralVacancies;
        existing.SelectionPatternSummary = request.SelectionPatternSummary;
        existing.OfficialSourceUrl = request.OfficialSourceUrl;
        existing.Notes = request.Notes;

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("HistoricalRecruitment", existing.Id.ToString(), "AddOrUpdateHistory", null, request, $"Cycle {request.CycleYear}", cancellationToken);
    }

    public async Task AddSyllabusMappingAsync(Guid examId, AdminAddSyllabusMappingRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _db.Exams.AnyAsync(e => e.Id == examId, cancellationToken))
            throw new NotFoundException("Exam not found.");
        if (!await _db.SyllabusSubjects.AnyAsync(s => s.Id == request.SyllabusSubjectId, cancellationToken))
            throw new NotFoundException("Syllabus subject not found.");

        var exists = await _db.ExamSyllabi.FirstOrDefaultAsync(es =>
            es.ExamId == examId && es.SyllabusSubjectId == request.SyllabusSubjectId && es.StageName == request.StageName, cancellationToken);

        if (exists is null)
        {
            exists = new ExamSyllabus
            {
                ExamId = examId,
                SyllabusSubjectId = request.SyllabusSubjectId,
                StageName = request.StageName.Trim(),
                DisplayOrder = await _db.ExamSyllabi.CountAsync(x => x.ExamId == examId, cancellationToken) + 1
            };
            _db.ExamSyllabi.Add(exists);
        }

        exists.WeightageNotes = request.WeightageNotes;
        exists.ExamSpecificTopicsNotes = request.ExamSpecificTopicsNotes;
        exists.OfficialSyllabusReferenceUrl = request.OfficialSyllabusReferenceUrl;

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("ExamSyllabus", exists.Id.ToString(), "UpsertSyllabusMapping", null, request, null, cancellationToken);
    }

    public async Task ChangeExamStatusAsync(Guid examId, AdminChangeStatusRequest request, CancellationToken cancellationToken = default)
    {
        var notif = await _db.ExamNotifications.FirstOrDefaultAsync(n => n.ExamId == examId && n.IsCurrentCycle, cancellationToken)
            ?? throw new NotFoundException("Current notification not found.");

        var oldStatus = notif.Status;
        notif.Status = request.Status;
        notif.StatusReason = request.Reason.Trim();
        notif.LastChangedAtUtc = DateTime.UtcNow;
        notif.LastChangeSummary = $"Status changed from {oldStatus} to {request.Status}: {request.Reason}";

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("ExamNotification", notif.Id.ToString(), request.Status.ToString(), new { oldStatus }, new { request.Status }, request.Reason, cancellationToken);
    }

    public async Task ApprovePendingRecordAsync(Guid examId, AdminApproveRecordRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams.Include(e => e.Notifications).FirstOrDefaultAsync(e => e.Id == examId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        exam.IsApprovedForPublish = request.Approve;
        exam.VerificationStatus = request.VerificationStatus;
        exam.DataConfidence = request.DataConfidence;

        foreach (var n in exam.Notifications.Where(n => n.IsCurrentCycle))
        {
            n.IsPendingApproval = !request.Approve;
            n.VerificationStatus = request.VerificationStatus;
            n.DataConfidence = request.DataConfidence;
            n.LastVerifiedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("Exam", exam.Id.ToString(), request.Approve ? "Approve" : "Reject", null, request, request.Notes, cancellationToken);
    }

    public async Task<string> UploadNotificationPdfAsync(Guid examId, string fileName, Stream contentStream, CancellationToken cancellationToken = default)
    {
        var notif = await _db.ExamNotifications.FirstOrDefaultAsync(n => n.ExamId == examId && n.IsCurrentCycle, cancellationToken)
            ?? throw new NotFoundException("Active exam notification cycle not found.");

        var uploadsDir = Path.Combine(AppContext.BaseDirectory, "wwwroot", "notifications");
        Directory.CreateDirectory(uploadsDir);

        var safeFileName = $"{examId:N}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(uploadsDir, safeFileName);
        await using (var fs = File.Create(fullPath))
        {
            await contentStream.CopyToAsync(fs, cancellationToken);
        }

        var relativeUrl = $"/notifications/{safeFileName}";
        notif.OfficialNotificationPdfUrl = relativeUrl;
        notif.LastChangedAtUtc = DateTime.UtcNow;
        notif.LastChangeSummary = $"Official notification PDF uploaded ({safeFileName}).";

        await _db.SaveChangesAsync(cancellationToken);
        await _audit.LogAsync("ExamNotification", notif.Id.ToString(), "UploadPdf", null, new { relativeUrl }, fileName, cancellationToken);
        return relativeUrl;
    }

    public async Task<IReadOnlyList<AdminOfficialSourceDto>> GetSourcesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.OfficialSources
            .AsNoTracking()
            .Include(s => s.Organization)
            .OrderBy(s => s.Organization.ShortCode)
            .Select(s => new AdminOfficialSourceDto
            {
                Id = s.Id,
                OrganizationName = s.Organization.Name,
                OrganizationCode = s.Organization.ShortCode,
                SourceName = s.SourceName,
                OfficialSourceUrl = s.OfficialSourceUrl,
                CalendarOrFeedUrl = s.CalendarOrFeedUrl,
                DateLastCheckedUtc = s.DateLastCheckedUtc,
                DateLastUpdatedUtc = s.DateLastUpdatedUtc,
                VerificationStatus = s.VerificationStatus,
                DataConfidence = s.DataConfidence,
                LastHttpStatusCode = s.LastHttpStatusCode,
                IsActive = s.IsActive,
                Notes = s.Notes
            }).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AdminAuditLogDto>> GetAuditLogsAsync(int take = 100, CancellationToken cancellationToken = default)
    {
        return await _db.AuditLogs
            .AsNoTracking()
            .OrderByDescending(a => a.TimestampUtc)
            .Take(Math.Clamp(take, 1, 500))
            .Select(a => new AdminAuditLogDto
            {
                Id = a.Id,
                UserEmail = a.UserEmail,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                OldValuesJson = a.OldValuesJson,
                NewValuesJson = a.NewValuesJson,
                Reason = a.Reason,
                TimestampUtc = a.TimestampUtc
            }).ToListAsync(cancellationToken);
    }
}
