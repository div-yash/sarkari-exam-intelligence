using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Eligibility;
using SarkariExamIntelligence.Application.DTOs.Exams;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class ExamQueryService : IExamQueryService
{
    private readonly IApplicationDbContext _db;
    private readonly IEligibilityEngine _eligibilityEngine;
    private readonly IDateTimeProvider _dateTime;

    public ExamQueryService(
        IApplicationDbContext db,
        IEligibilityEngine eligibilityEngine,
        IDateTimeProvider dateTime)
    {
        _db = db;
        _eligibilityEngine = eligibilityEngine;
        _dateTime = dateTime;
    }

    public async Task<IReadOnlyList<CategorySummaryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategorySummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Code = c.Code,
                Description = c.Description,
                ExamCount = c.Exams.Count(e => e.IsApprovedForPublish && !e.IsDeleted)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<ExamListItemDto>> SearchExamsAsync(
        ExamFilterRequest filter,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var today = _dateTime.TodayIst;

        var query = _db.Exams
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsApprovedForPublish)
            .Include(e => e.Category)
            .Include(e => e.Organization)
            .Include(e => e.OfficialSource)
            .Include(e => e.Notifications)
                .ThenInclude(n => n.Vacancy)
            .Include(e => e.EligibilityRules)
            .Include(e => e.AgeRelaxations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.CategorySlug))
            query = query.Where(e => e.Category.Slug == filter.CategorySlug);

        if (filter.OnlyBihar == true)
            query = query.Where(e => e.IsBiharSpecific);

        if (filter.OnlyItSpecific == true)
            query = query.Where(e => e.IsItSpecific);

        if (filter.OnlyGraduateLevel == true)
            query = query.Where(e => e.IsGraduateLevel);

        if (!string.IsNullOrWhiteSpace(filter.State))
        {
            var st = filter.State.Trim().ToLower();
            query = query.Where(e =>
                (e.Organization.StateName != null && e.Organization.StateName.ToLower().Contains(st)) ||
                (st == "bihar" && e.IsBiharSpecific) ||
                e.Organization.OrganizationType == "Central");
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.Trim().ToLower();
            query = query.Where(e =>
                e.Name.ToLower().Contains(s) ||
                e.ShortName.ToLower().Contains(s) ||
                e.Department.ToLower().Contains(s) ||
                e.JobDesignation.ToLower().Contains(s) ||
                e.Organization.Name.ToLower().Contains(s) ||
                e.Organization.ShortCode.ToLower().Contains(s) ||
                e.EligibilityRules.Any(r =>
                    r.QualificationSummary.ToLower().Contains(s) ||
                    (r.AcceptedEngineeringBranchesCsv != null && r.AcceptedEngineeringBranchesCsv.ToLower().Contains(s))));
        }

        if (!string.IsNullOrWhiteSpace(filter.Qualification))
        {
            var q = filter.Qualification.Trim().ToLower();
            query = query.Where(e => e.EligibilityRules.Any(r =>
                r.QualificationSummary.ToLower().Contains(q) ||
                r.MinimumQualificationLevel.ToLower().Contains(q)));
        }

        if (!string.IsNullOrWhiteSpace(filter.EngineeringBranch))
        {
            var br = filter.EngineeringBranch.Trim().ToLower();
            query = query.Where(e => e.EligibilityRules.Any(r =>
                r.AcceptsAnyGraduateDegree ||
                (r.AcceptedEngineeringBranchesCsv != null && r.AcceptedEngineeringBranchesCsv.ToLower().Contains(br))));
        }

        if (filter.MaxAgeLimit.HasValue)
        {
            query = query.Where(e => e.EligibilityRules.Any(r => !r.MaxAgeYears.HasValue || r.MaxAgeYears >= filter.MaxAgeLimit));
        }

        var exams = await query.ToListAsync(cancellationToken);

        UserProfile? profile = null;
        HashSet<Guid> savedIds = new();
        if (userId.HasValue)
        {
            profile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId.Value, cancellationToken);
            savedIds = (await _db.SavedExams
                .AsNoTracking()
                .Where(s => s.UserId == userId.Value)
                .Select(s => s.ExamId)
                .ToListAsync(cancellationToken))
                .ToHashSet();
        }

        var items = new List<ExamListItemDto>();
        foreach (var exam in exams)
        {
            var current = exam.Notifications.FirstOrDefault(n => n.IsCurrentCycle)
                ?? exam.Notifications.OrderByDescending(n => n.CycleYear).FirstOrDefault();

            var status = current?.Status ?? ExamStatus.UpcomingNotification;
            if (filter.Status.HasValue && status != filter.Status.Value)
                continue;

            if (filter.NotificationYear.HasValue && current?.CycleYear != filter.NotificationYear.Value)
                continue;

            if (filter.OnlyOpenApplications == true && status is not (ExamStatus.ApplicationsOpen or ExamStatus.ClosingSoon))
                continue;

            var notifDate = current?.NotificationDate ?? current?.ExpectedNotificationDate;
            if (filter.NotificationsExpectedThisMonth == true)
            {
                if (notifDate is null || notifDate.Value.Year != today.Year || notifDate.Value.Month != today.Month)
                    continue;
            }

            if (filter.NotificationsExpectedIn3Months == true)
            {
                if (notifDate is null || notifDate.Value < today || notifDate.Value > today.AddMonths(3))
                    continue;
            }

            var examDate = current?.ExamDate ?? current?.ExpectedExamDate ?? current?.PrelimExamDate ?? current?.ExpectedPrelimExamDate;
            if (filter.ExamsIn30Days == true)
            {
                if (examDate is null || examDate.Value < today || examDate.Value > today.AddDays(30))
                    continue;
            }

            if (filter.ExamsIn90Days == true)
            {
                if (examDate is null || examDate.Value < today || examDate.Value > today.AddDays(90))
                    continue;
            }

            EligibilityCheckResultDto? elig = null;
            if (profile is not null)
                elig = _eligibilityEngine.EvaluateExam(exam, current, profile, today);

            if (filter.OnlyMatchingMyEligibility == true &&
                (elig is null || elig.Status == EligibilityResultStatus.NotEligible))
                continue;

            var rule = current?.EligibilityRules.FirstOrDefault() ?? exam.EligibilityRules.FirstOrDefault();
            var deadline = current?.ApplicationEndDate ?? current?.ExpectedApplicationEndDate;
            int? daysToDeadline = deadline.HasValue ? deadline.Value.DayNumber - today.DayNumber : null;

            items.Add(new ExamListItemDto
            {
                Id = exam.Id,
                CurrentNotificationId = current?.Id,
                Name = exam.Name,
                ShortName = exam.ShortName,
                Slug = exam.Slug,
                Code = exam.Code,
                CategoryName = exam.Category.Name,
                CategorySlug = exam.Category.Slug,
                OrganizationName = exam.Organization.Name,
                OrganizationCode = exam.Organization.ShortCode,
                Department = exam.Department,
                JobDesignation = exam.JobDesignation,
                GroupClassification = exam.GroupClassification,
                JobNature = exam.JobNature,
                IsBiharSpecific = exam.IsBiharSpecific,
                IsItSpecific = exam.IsItSpecific,
                IsGraduateLevel = exam.IsGraduateLevel,
                CycleYear = current?.CycleYear,
                Status = status,
                NotificationDate = current?.NotificationDate,
                ExpectedNotificationDate = current?.ExpectedNotificationDate,
                IsNotificationDateExpected = current?.IsNotificationDateExpected ?? false,
                ApplicationStartDate = current?.ApplicationStartDate,
                ExpectedApplicationStartDate = current?.ExpectedApplicationStartDate,
                IsApplicationStartExpected = current?.IsApplicationStartExpected ?? false,
                ApplicationEndDate = current?.ApplicationEndDate,
                ExpectedApplicationEndDate = current?.ExpectedApplicationEndDate,
                IsApplicationEndExpected = current?.IsApplicationEndExpected ?? false,
                ExamDate = current?.ExamDate ?? current?.PrelimExamDate,
                ExpectedExamDate = current?.ExpectedExamDate ?? current?.ExpectedPrelimExamDate,
                IsExamDateExpected = (current?.IsExamDateExpected ?? false) || (current?.IsPrelimDateExpected ?? false),
                DaysToDeadline = daysToDeadline,
                TotalVacancies = current?.Vacancy?.TotalVacancies,
                ItSpecificVacancies = current?.Vacancy?.ItSpecificVacancies,
                VacancyType = current?.Vacancy?.VacancyType ?? VacancyType.NotAnnounced,
                MinAge = rule?.MinAgeYears,
                MaxAge = rule?.MaxAgeYears,
                QualificationSummary = rule?.QualificationSummary ?? "Not Announced",
                VerificationStatus = current?.VerificationStatus ?? exam.VerificationStatus,
                DataConfidence = current?.DataConfidence ?? exam.DataConfidence,
                OfficialRecruitmentWebsite = exam.OfficialRecruitmentWebsite,
                OfficialNotificationPdfUrl = current?.OfficialNotificationPdfUrl,
                OfficialApplicationUrl = current?.OfficialApplicationUrl,
                IsSaved = savedIds.Contains(exam.Id),
                UserEligibilityStatus = elig?.Status,
                UserEligibilitySummary = elig?.Summary
            });
        }

        IEnumerable<ExamListItemDto> sorted = filter.SortBy.ToLowerInvariant() switch
        {
            "notificationdate" => filter.SortDescending
                ? items.OrderByDescending(i => i.NotificationDate ?? i.ExpectedNotificationDate ?? DateOnly.MinValue)
                : items.OrderBy(i => i.NotificationDate ?? i.ExpectedNotificationDate ?? DateOnly.MaxValue),
            "examdate" => filter.SortDescending
                ? items.OrderByDescending(i => i.ExamDate ?? i.ExpectedExamDate ?? DateOnly.MinValue)
                : items.OrderBy(i => i.ExamDate ?? i.ExpectedExamDate ?? DateOnly.MaxValue),
            "vacancies" => filter.SortDescending
                ? items.OrderByDescending(i => i.TotalVacancies ?? -1)
                : items.OrderBy(i => i.TotalVacancies ?? int.MaxValue),
            "eligibility" => items
                .OrderBy(i => i.UserEligibilityStatus.HasValue ? (int)i.UserEligibilityStatus.Value : 99)
                .ThenBy(i => i.ApplicationEndDate ?? i.ExpectedApplicationEndDate ?? DateOnly.MaxValue),
            _ => filter.SortDescending
                ? items.OrderByDescending(i => i.ApplicationEndDate ?? i.ExpectedApplicationEndDate ?? DateOnly.MinValue)
                : items.OrderBy(i => i.Status is ExamStatus.ClosingSoon or ExamStatus.ApplicationsOpen ? 0 : 1)
                       .ThenBy(i => i.ApplicationEndDate ?? i.ExpectedApplicationEndDate ?? DateOnly.MaxValue)
        };

        var totalCount = sorted.Count();
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);
        var pagedItems = sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<ExamListItemDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ExamDetailDto> GetExamDetailBySlugAsync(string slug, Guid? userId, CancellationToken cancellationToken = default)
    {
        var exam = await LoadExamDetailQuery()
            .FirstOrDefaultAsync(e => e.Slug == slug && !e.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Exam '{slug}' was not found.");

        return await MapExamDetailAsync(exam, userId, cancellationToken);
    }

    public async Task<ExamDetailDto> GetExamDetailByIdAsync(Guid examId, Guid? userId, CancellationToken cancellationToken = default)
    {
        var exam = await LoadExamDetailQuery()
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        return await MapExamDetailAsync(exam, userId, cancellationToken);
    }

    private IQueryable<Exam> LoadExamDetailQuery() =>
        _db.Exams
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Organization)
            .Include(e => e.OfficialSource)
            .Include(e => e.Notifications)
                .ThenInclude(n => n.Vacancy!)
                    .ThenInclude(v => v.CategoryWiseVacancies)
            .Include(e => e.Notifications)
                .ThenInclude(n => n.ExamDates)
            .Include(e => e.EligibilityRules)
            .Include(e => e.EducationalQualifications)
            .Include(e => e.AgeRelaxations)
            .Include(e => e.ExamPatterns)
            .Include(e => e.ExamSyllabi)
                .ThenInclude(es => es.SyllabusSubject)
                    .ThenInclude(ss => ss.Topics)
            .Include(e => e.HistoricalRecruitments)
                .ThenInclude(h => h.CategoryVacancies)
            .Include(e => e.PreviousYearCutoffs)
            .Include(e => e.PreviousYearPapers);

    private async Task<ExamDetailDto> MapExamDetailAsync(Exam exam, Guid? userId, CancellationToken cancellationToken)
    {
        var current = exam.Notifications.FirstOrDefault(n => n.IsCurrentCycle)
            ?? exam.Notifications.OrderByDescending(n => n.CycleYear).FirstOrDefault();

        var rule = current?.EligibilityRules.FirstOrDefault()
            ?? exam.EligibilityRules.FirstOrDefault();

        var isSaved = false;
        string? appStatus = null;
        if (userId.HasValue)
        {
            isSaved = await _db.SavedExams.AnyAsync(s => s.UserId == userId.Value && s.ExamId == exam.Id, cancellationToken);
            var app = await _db.UserApplications.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId.Value && a.ExamId == exam.Id, cancellationToken);
            appStatus = app?.TrackingStatus.ToString();
        }

        return new ExamDetailDto
        {
            Id = exam.Id,
            CurrentNotificationId = current?.Id,
            Name = exam.Name,
            ShortName = exam.ShortName,
            Slug = exam.Slug,
            Code = exam.Code,
            CategoryName = exam.Category.Name,
            CategorySlug = exam.Category.Slug,
            ConductingOrganization = exam.Organization.Name,
            OrganizationShortCode = exam.Organization.ShortCode,
            Department = exam.Department,
            JobDesignation = exam.JobDesignation,
            GroupClassification = exam.GroupClassification,
            JobNature = exam.JobNature,
            Summary = exam.Summary,
            IsBiharSpecific = exam.IsBiharSpecific,
            IsItSpecific = exam.IsItSpecific,
            IsGraduateLevel = exam.IsGraduateLevel,

            CycleYear = current?.CycleYear,
            NotificationNumber = current?.NotificationNumber,
            Status = current?.Status ?? ExamStatus.UpcomingNotification,
            StatusReason = current?.StatusReason,

            NotificationPublicationDate = BuildDateField(current?.NotificationDate, current?.ExpectedNotificationDate, current?.IsNotificationDateExpected ?? false),
            ApplicationOpeningDate = BuildDateField(current?.ApplicationStartDate, current?.ExpectedApplicationStartDate, current?.IsApplicationStartExpected ?? false),
            ApplicationClosingDate = BuildDateField(current?.ApplicationEndDate, current?.ExpectedApplicationEndDate, current?.IsApplicationEndExpected ?? false),
            CorrectionWindowStart = current?.CorrectionWindowStart,
            CorrectionWindowEnd = current?.CorrectionWindowEnd,
            AdmitCardDate = BuildDateField(current?.AdmitCardDate, current?.ExpectedAdmitCardDate, current?.IsAdmitCardExpected ?? false),
            PreliminaryExamDate = BuildDateField(current?.PrelimExamDate ?? current?.ExamDate, current?.ExpectedPrelimExamDate ?? current?.ExpectedExamDate, (current?.IsPrelimDateExpected ?? false) || (current?.IsExamDateExpected ?? false)),
            MainsExamDate = BuildDateField(current?.MainsExamDate, current?.ExpectedMainsExamDate, current?.IsMainsDateExpected ?? false),
            InterviewDate = BuildDateField(current?.InterviewDate, current?.ExpectedInterviewDate, current?.IsInterviewDateExpected ?? false),
            ResultDate = BuildDateField(current?.ResultDate, current?.ExpectedResultDate, current?.IsResultDateExpected ?? false),

            EligibilityRule = rule is null ? null : new EligibilityRuleDetailDto
            {
                MinAgeYears = rule.MinAgeYears,
                MaxAgeYears = rule.MaxAgeYears,
                AgeCutoffDate = rule.AgeCutoffDate ?? current?.AgeCutoffDate,
                AgeCutoffBasisDescription = rule.AgeCutoffBasisDescription,
                MinimumQualificationLevel = rule.MinimumQualificationLevel,
                RequiresEngineeringOrTechDegree = rule.RequiresEngineeringOrTechDegree,
                AcceptsAnyGraduateDegree = rule.AcceptsAnyGraduateDegree,
                AcceptedEngineeringBranches = rule.AcceptedEngineeringBranchesCsv,
                QualificationSummary = rule.QualificationSummary,
                MinPercentageGeneral = rule.MinPercentageGeneral,
                MinPercentageReserved = rule.MinPercentageReserved,
                MinCgpa = rule.MinCgpa,
                FinalYearAppearingEligible = rule.FinalYearAppearingEligible,
                RequiresWorkExperience = rule.RequiresWorkExperience,
                MinWorkExperienceMonths = rule.MinWorkExperienceMonths,
                ExperienceRequirementNotes = rule.ExperienceRequirementNotes,
                NationalityRequirement = rule.NationalityRequirement,
                DomicileRequirement = rule.DomicileRequirement,
                BiharDomicileRequiredForReservation = rule.BiharDomicileRequiredForReservation,
                BiharCertificateConditions = rule.BiharCertificateConditions,
                GenderRequirement = rule.GenderRequirement,
                RequiresValidGateScore = rule.RequiresValidGateScore,
                AcceptedGatePapers = rule.AcceptedGatePapersCsv,
                MinGateScore = rule.MinGateScore,
                AdditionalConditions = rule.AdditionalConditions,
                VerificationNotes = rule.VerificationNotes,
                SourceReference = rule.SourceReference
            },

            EducationalQualifications = exam.EducationalQualifications.Select(q => new EducationalQualificationDto
            {
                DegreeLevel = q.DegreeLevel,
                DegreeName = q.DegreeName,
                AcceptedBranches = q.AcceptedBranchesCsv,
                MinimumPercentage = q.MinimumPercentage,
                IsMandatory = q.IsMandatory,
                Notes = q.Notes
            }).ToList(),

            AgeRelaxations = exam.AgeRelaxations.Select(a => new AgeRelaxationDto
            {
                Category = a.Category,
                SubCategory = a.SubCategory,
                RelaxationYears = a.RelaxationYears,
                MaxUpperAgeOverride = a.MaxUpperAgeOverride,
                RequiresDomicileForStateQuota = a.RequiresDomicileForStateQuota,
                IsSupportedByNotification = a.IsSupportedByNotification,
                Conditions = a.Conditions
            }).ToList(),

            Vacancy = current?.Vacancy is null ? null : new ExamVacancyDetailDto
            {
                TotalVacancies = current.Vacancy.TotalVacancies,
                ItSpecificVacancies = current.Vacancy.ItSpecificVacancies,
                GeneralVacancies = current.Vacancy.GeneralVacancies,
                VacancyType = current.Vacancy.VacancyType,
                Notes = current.Vacancy.Notes,
                SourceReference = current.Vacancy.SourceReference,
                CategoryWiseVacancies = current.Vacancy.CategoryWiseVacancies.Select(cv => new CategoryVacancyItemDto
                {
                    Category = cv.Category,
                    SubCategory = cv.SubCategory,
                    VacancyCount = cv.VacancyCount,
                    Notes = cv.Notes
                }).ToList()
            },

            ApplicationFee = current?.ApplicationFeeSummary ?? "Not Announced",
            SalaryPayLevel = current?.SalaryPayLevel ?? exam.DefaultSalaryPayLevel ?? "Not Announced",
            SelectionProcedure = current?.SelectionProcedure ?? exam.DefaultSelectionProcedure ?? "Not Announced",
            ExamMode = current?.ExamMode ?? exam.DefaultExamMode,
            NegativeMarking = current?.NegativeMarkingDetails ?? exam.DefaultNegativeMarking ?? "Not Announced",

            ExamPatterns = exam.ExamPatterns
                .OrderBy(p => p.OrderIndex)
                .Select(p => new ExamPatternDto
                {
                    StageName = p.StageName,
                    SectionName = p.SectionName,
                    QuestionsCount = p.QuestionsCount,
                    MaxMarks = p.MaxMarks,
                    DurationMinutes = p.DurationMinutes,
                    NegativeMarkingDetails = p.NegativeMarkingDetails,
                    IsQualifyingOnly = p.IsQualifyingOnly
                }).ToList(),

            Syllabi = exam.ExamSyllabi
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new ExamSyllabusMappingDto
                {
                    SubjectId = s.SyllabusSubjectId,
                    SubjectCode = s.SyllabusSubject.Code,
                    SubjectName = s.SyllabusSubject.Name,
                    SubjectGroup = s.SyllabusSubject.SubjectGroup,
                    StageName = s.StageName,
                    WeightageNotes = s.WeightageNotes,
                    ExamSpecificTopicsNotes = s.ExamSpecificTopicsNotes,
                    OfficialSyllabusReferenceUrl = s.OfficialSyllabusReferenceUrl,
                    Topics = s.SyllabusSubject.Topics.OrderBy(t => t.DisplayOrder).Select(t => t.Name).ToList()
                }).ToList(),

            HistoricalRecruitments = exam.HistoricalRecruitments
                .OrderBy(h => h.CycleYear)
                .Select(h => new HistoricalRecruitmentDto
                {
                    CycleYear = h.CycleYear,
                    DataStatus = h.DataStatus,
                    NotificationDate = h.NotificationDate,
                    ApplicationStartDate = h.ApplicationStartDate,
                    ApplicationEndDate = h.ApplicationEndDate,
                    PrelimExamDate = h.PrelimExamDate,
                    MainsExamDate = h.MainsExamDate,
                    ResultDate = h.ResultDate,
                    TotalVacancies = h.TotalVacancies,
                    ItVacancies = h.ItVacancies,
                    GeneralVacancies = h.GeneralVacancies,
                    SelectionPatternSummary = h.SelectionPatternSummary,
                    OfficialSourceUrl = h.OfficialSourceUrl,
                    Notes = h.Notes,
                    CategoryVacancies = h.CategoryVacancies.Select(cv => new CategoryVacancyItemDto
                    {
                        Category = cv.Category,
                        VacancyCount = cv.VacancyCount,
                        Notes = cv.Notes
                    }).ToList()
                }).ToList(),

            PreviousYearCutoffs = exam.PreviousYearCutoffs
                .OrderByDescending(c => c.CycleYear)
                .Select(c => new PreviousYearCutoffDto
                {
                    CycleYear = c.CycleYear,
                    StageName = c.StageName,
                    Category = c.Category,
                    CutoffMarks = c.CutoffMarks,
                    TotalMaxMarks = c.TotalMaxMarks,
                    SourceUrl = c.SourceUrl,
                    Notes = c.Notes
                }).ToList(),

            PreviousYearPapers = exam.PreviousYearPapers
                .OrderByDescending(p => p.CycleYear)
                .Select(p => new PreviousYearPaperDto
                {
                    CycleYear = p.CycleYear,
                    StageName = p.StageName,
                    Title = p.Title,
                    PaperUrl = p.PaperUrl,
                    SourceLabel = p.SourceLabel,
                    IsOfficial = p.IsOfficial
                }).ToList(),

            OfficialNotificationPdfUrl = current?.OfficialNotificationPdfUrl,
            OfficialApplicationUrl = current?.OfficialApplicationUrl,
            OfficialRecruitmentWebsite = exam.OfficialRecruitmentWebsite,
            OfficialSource = new OfficialSourceTraceDto
            {
                SourceOrganization = exam.Organization.Name,
                OfficialSourceUrl = exam.OfficialSource.OfficialSourceUrl,
                NotificationPdfUrl = current?.OfficialNotificationPdfUrl ?? exam.OfficialSource.LastNotificationPdfUrl,
                DateLastCheckedUtc = exam.OfficialSource.DateLastCheckedUtc ?? current?.LastVerifiedAtUtc,
                DateLastUpdatedUtc = exam.OfficialSource.DateLastUpdatedUtc ?? current?.LastChangedAtUtc,
                SourcePublicationDate = current?.SourcePublicationDate ?? exam.OfficialSource.SourcePublicationDate,
                VerificationStatus = current?.VerificationStatus ?? exam.VerificationStatus,
                DataConfidence = current?.DataConfidence ?? exam.DataConfidence,
                LastChangeSummary = current?.LastChangeSummary
            },

            IsSaved = isSaved,
            UserApplicationStatus = appStatus
        };
    }

    private static DateFieldDto BuildDateField(DateOnly? confirmed, DateOnly? expected, bool isExpectedFlag)
    {
        if (confirmed.HasValue)
        {
            return new DateFieldDto
            {
                ConfirmedDate = confirmed,
                ExpectedDate = expected,
                IsExpected = false,
                DisplayText = confirmed.Value.ToString("dd MMM yyyy"),
                DateLabelType = "Confirmed"
            };
        }

        if (expected.HasValue)
        {
            return new DateFieldDto
            {
                ConfirmedDate = null,
                ExpectedDate = expected,
                IsExpected = true,
                DisplayText = $"Expected: {expected.Value:MMM yyyy}",
                DateLabelType = "Expected"
            };
        }

        return new DateFieldDto
        {
            ConfirmedDate = null,
            ExpectedDate = null,
            IsExpected = isExpectedFlag,
            DisplayText = "Not Announced",
            DateLabelType = "Not Announced"
        };
    }
}
