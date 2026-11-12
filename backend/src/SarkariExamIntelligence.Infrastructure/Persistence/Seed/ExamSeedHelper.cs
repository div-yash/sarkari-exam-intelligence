using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Seed;

public class ExamSeedSpec
{
    public required string Code { get; init; }
    public required string Slug { get; init; }
    public required string Name { get; init; }
    public required string ShortName { get; init; }
    public required string CategorySlug { get; init; }
    public required string OrgCode { get; init; }
    public required string Department { get; init; }
    public required string Designation { get; init; }
    public GroupClassification Group { get; init; } = GroupClassification.GroupA;
    public JobNature Nature { get; init; } = JobNature.Permanent;
    public required string OfficialWebsite { get; init; }
    public bool IsBihar { get; init; }
    public bool IsIt { get; init; }
    public bool IsGraduate { get; init; } = true;
    public string Summary { get; init; } = string.Empty;

    // Cycle & Notification
    public int CycleYear { get; init; } = 2026;
    public string? NotificationNumber { get; init; }
    public ExamStatus Status { get; init; } = ExamStatus.UpcomingNotification;
    public string? StatusReason { get; init; }
    public DateOnly? NotificationDate { get; init; }
    public DateOnly? ExpectedNotificationDate { get; init; }
    public DateOnly? AppStart { get; init; }
    public DateOnly? ExpectedAppStart { get; init; }
    public DateOnly? AppEnd { get; init; }
    public DateOnly? ExpectedAppEnd { get; init; }
    public DateOnly? CorrectionStart { get; init; }
    public DateOnly? CorrectionEnd { get; init; }
    public DateOnly? AdmitCardDate { get; init; }
    public DateOnly? ExpectedAdmitCardDate { get; init; }
    public DateOnly? PrelimDate { get; init; }
    public DateOnly? ExpectedPrelimDate { get; init; }
    public DateOnly? MainsDate { get; init; }
    public DateOnly? ExpectedMainsDate { get; init; }
    public DateOnly? InterviewDate { get; init; }
    public DateOnly? ResultDate { get; init; }
    public DateOnly? ExpectedResultDate { get; init; }
    public DateOnly? AgeCutoffDate { get; init; }
    public string AgeCutoffBasis { get; init; } = "As specified in the official recruitment notification";
    public string? PdfUrl { get; init; }
    public string? ApplyUrl { get; init; }
    public DataConfidence Confidence { get; init; } = DataConfidence.ConfirmedOfficial;
    public VerificationStatus VerificationStatus { get; init; } = VerificationStatus.VerifiedOfficial;

    // Details
    public string FeeSummary { get; init; } = "Refer to official notification PDF";
    public string SalaryPayLevel { get; init; } = "7th CPC Pay Matrix";
    public string SelectionProcedure { get; init; } = "Written Examination + Document Verification";
    public ExamMode ExamMode { get; init; } = ExamMode.ComputerBasedTest;
    public string NegativeMarking { get; init; } = "Refer to official exam scheme";

    // Vacancy
    public int? TotalVacancies { get; init; }
    public int? ItVacancies { get; init; }
    public VacancyType VacancyType { get; init; } = VacancyType.NotAnnounced;
    public string? VacancyNotes { get; init; }
    public List<(CategoryType Cat, int Count)>? CategoryVacancies { get; init; }

    // Eligibility
    public int MinAge { get; init; } = 21;
    public int? MaxAge { get; init; } = 30;
    public string MinQualLevel { get; init; } = "Graduate";
    public bool RequiresEngineering { get; init; }
    public bool AcceptsAnyGraduate { get; init; } = true;
    public string? AcceptedBranches { get; init; }
    public string QualificationSummary { get; init; } = "Bachelor's degree from a recognized university";
    public decimal? MinPctGen { get; init; }
    public decimal? MinPctRes { get; init; }
    public bool FinalYearEligible { get; init; }
    public bool RequiresExp { get; init; }
    public int MinExpMonths { get; init; }
    public string? ExpNotes { get; init; }
    public string? DomicileReq { get; init; } = "All India candidates eligible";
    public bool BiharDomicileForReservation { get; init; }
    public string? BiharCertConditions { get; init; }
    public string? GenderReq { get; init; }
    public bool RequiresGate { get; init; }
    public string? AcceptedGatePapers { get; init; }
    public string? VerificationNotes { get; init; }
    public int ObcRelaxation { get; init; } = 3;
    public int ScStRelaxation { get; init; } = 5;
    public int PwbdRelaxation { get; init; } = 10;
    public bool SupportRelaxations { get; init; } = true;

    // Pattern, Syllabus, History, Cutoffs
    public string[] SyllabusSubjectCodes { get; init; } = Array.Empty<string>();
    public List<(string Stage, string Section, int? Qs, decimal? Marks, int? Mins, string? Neg, bool Qual)>? Patterns { get; init; }
    public List<(int Year, HistoricalDataStatus Status, int? Total, int? It, DateOnly? NotifDate, DateOnly? ExamDate, string? Notes)>? History { get; init; }
    public List<(int Year, string Stage, CategoryType Cat, decimal? Marks, decimal? Max)>? Cutoffs { get; init; }
}

public static class ExamSeedHelper
{
    public static Exam BuildExam(
        ExamSeedSpec s,
        OrgSourceSeedResult refs,
        Dictionary<string, SyllabusSubject> subjects)
    {
        var category = refs.Categories[s.CategorySlug];
        var org = refs.Organizations[s.OrgCode];
        var source = refs.Sources[s.OrgCode];

        var exam = new Exam
        {
            CategoryId = category.Id,
            OrganizationId = org.Id,
            OfficialSourceId = source.Id,
            Name = s.Name,
            ShortName = s.ShortName,
            Slug = s.Slug,
            Code = s.Code,
            Department = s.Department,
            JobDesignation = s.Designation,
            GroupClassification = s.Group,
            JobNature = s.Nature,
            OfficialRecruitmentWebsite = s.OfficialWebsite,
            IsBiharSpecific = s.IsBihar,
            IsItSpecific = s.IsIt,
            IsGraduateLevel = s.IsGraduate,
            VerificationStatus = s.VerificationStatus,
            DataConfidence = s.Confidence,
            IsApprovedForPublish = true,
            Summary = string.IsNullOrWhiteSpace(s.Summary) ? $"{s.Name} conducted by {org.Name} for {s.Designation}." : s.Summary,
            DefaultSalaryPayLevel = s.SalaryPayLevel,
            DefaultSelectionProcedure = s.SelectionProcedure,
            DefaultExamMode = s.ExamMode,
            DefaultNegativeMarking = s.NegativeMarking
        };

        var notif = new ExamNotification
        {
            CycleYear = s.CycleYear,
            NotificationNumber = s.NotificationNumber,
            Title = $"{s.Name} ({s.CycleYear})",
            Status = s.Status,
            StatusReason = s.StatusReason,
            IsCurrentCycle = true,
            NotificationDate = s.NotificationDate,
            ExpectedNotificationDate = s.ExpectedNotificationDate,
            IsNotificationDateExpected = !s.NotificationDate.HasValue && s.ExpectedNotificationDate.HasValue,
            ApplicationStartDate = s.AppStart,
            ExpectedApplicationStartDate = s.ExpectedAppStart,
            IsApplicationStartExpected = !s.AppStart.HasValue && s.ExpectedAppStart.HasValue,
            ApplicationEndDate = s.AppEnd,
            ExpectedApplicationEndDate = s.ExpectedAppEnd,
            IsApplicationEndExpected = !s.AppEnd.HasValue && s.ExpectedAppEnd.HasValue,
            CorrectionWindowStart = s.CorrectionStart,
            CorrectionWindowEnd = s.CorrectionEnd,
            AdmitCardDate = s.AdmitCardDate,
            ExpectedAdmitCardDate = s.ExpectedAdmitCardDate,
            IsAdmitCardExpected = !s.AdmitCardDate.HasValue && s.ExpectedAdmitCardDate.HasValue,
            ExamDate = s.PrelimDate,
            ExpectedExamDate = s.ExpectedPrelimDate,
            IsExamDateExpected = !s.PrelimDate.HasValue && s.ExpectedPrelimDate.HasValue,
            PrelimExamDate = s.PrelimDate,
            ExpectedPrelimExamDate = s.ExpectedPrelimDate,
            IsPrelimDateExpected = !s.PrelimDate.HasValue && s.ExpectedPrelimDate.HasValue,
            MainsExamDate = s.MainsDate,
            ExpectedMainsExamDate = s.ExpectedMainsDate,
            IsMainsDateExpected = !s.MainsDate.HasValue && s.ExpectedMainsDate.HasValue,
            InterviewDate = s.InterviewDate,
            ResultDate = s.ResultDate,
            ExpectedResultDate = s.ExpectedResultDate,
            IsResultDateExpected = !s.ResultDate.HasValue && s.ExpectedResultDate.HasValue,
            AgeCutoffDate = s.AgeCutoffDate,
            AgeCutoffReferenceText = s.AgeCutoffBasis,
            ApplicationFeeSummary = s.FeeSummary,
            SalaryPayLevel = s.SalaryPayLevel,
            SelectionProcedure = s.SelectionProcedure,
            ExamMode = s.ExamMode,
            NegativeMarkingDetails = s.NegativeMarking,
            OfficialNotificationPdfUrl = s.PdfUrl ?? source.OfficialSourceUrl,
            OfficialApplicationUrl = s.ApplyUrl ?? org.RecruitmentPortalUrl ?? s.OfficialWebsite,
            SourcePublicationDate = s.NotificationDate,
            LastVerifiedAtUtc = DateTime.UtcNow,
            VerificationStatus = s.VerificationStatus,
            DataConfidence = s.Confidence,
            IsPendingApproval = false
        };

        var vacancy = new ExamVacancy
        {
            TotalVacancies = s.TotalVacancies,
            ItSpecificVacancies = s.ItVacancies ?? (s.IsIt ? s.TotalVacancies : null),
            GeneralVacancies = s.TotalVacancies.HasValue
                ? Math.Max(0, s.TotalVacancies.Value - (s.ItVacancies ?? (s.IsIt ? s.TotalVacancies.Value : 0)))
                : null,
            VacancyType = s.VacancyType,
            Notes = s.VacancyNotes ?? (s.VacancyType == VacancyType.NotAnnounced ? "Vacancy count not yet announced by recruiting body." : "Official notified count."),
            SourceReference = s.PdfUrl ?? source.OfficialSourceUrl
        };

        if (s.CategoryVacancies is not null)
        {
            foreach (var (cat, cnt) in s.CategoryVacancies)
            {
                vacancy.CategoryWiseVacancies.Add(new CategoryWiseVacancy { Category = cat, VacancyCount = cnt });
            }
        }
        notif.Vacancy = vacancy;

        var rule = new EligibilityRule
        {
            Exam = exam,
            ExamNotification = notif,
            MinAgeYears = s.MinAge,
            MaxAgeYears = s.MaxAge,
            AgeCutoffDate = s.AgeCutoffDate,
            AgeCutoffBasisDescription = s.AgeCutoffBasis,
            MinimumQualificationLevel = s.MinQualLevel,
            RequiresEngineeringOrTechDegree = s.RequiresEngineering,
            AcceptsAnyGraduateDegree = s.AcceptsAnyGraduate,
            AcceptedEngineeringBranchesCsv = s.AcceptedBranches,
            QualificationSummary = s.QualificationSummary,
            MinPercentageGeneral = s.MinPctGen,
            MinPercentageReserved = s.MinPctRes,
            FinalYearAppearingEligible = s.FinalYearEligible,
            RequiresWorkExperience = s.RequiresExp,
            MinWorkExperienceMonths = s.MinExpMonths,
            ExperienceRequirementNotes = s.ExpNotes,
            NationalityRequirement = "Citizen of India",
            DomicileRequirement = s.DomicileReq,
            BiharDomicileRequiredForReservation = s.BiharDomicileForReservation,
            BiharCertificateConditions = s.BiharCertConditions,
            GenderRequirement = s.GenderReq,
            RequiresValidGateScore = s.RequiresGate,
            AcceptedGatePapersCsv = s.AcceptedGatePapers,
            VerificationNotes = s.VerificationNotes,
            SourceReference = s.PdfUrl ?? source.OfficialSourceUrl
        };

        exam.Notifications.Add(notif);
        exam.EligibilityRules.Add(rule);
        exam.EducationalQualifications.Add(new EducationalQualification
        {
            DegreeLevel = s.MinQualLevel,
            DegreeName = s.QualificationSummary,
            AcceptedBranchesCsv = s.AcceptedBranches,
            MinimumPercentage = s.MinPctGen,
            IsMandatory = true
        });

        if (s.SupportRelaxations && s.MaxAge.HasValue)
        {
            exam.AgeRelaxations.Add(new AgeRelaxation
            {
                Category = CategoryType.OBCNCL,
                RelaxationYears = s.ObcRelaxation,
                RequiresDomicileForStateQuota = s.BiharDomicileForReservation,
                IsSupportedByNotification = true,
                Conditions = s.BiharDomicileForReservation
                    ? "Requires Bihar permanent domicile + valid Non-Creamy Layer (BC/EBC) certificate."
                    : "Requires valid Central OBC-NCL certificate."
            });
            exam.AgeRelaxations.Add(new AgeRelaxation
            {
                Category = CategoryType.SC,
                RelaxationYears = s.ScStRelaxation,
                RequiresDomicileForStateQuota = s.BiharDomicileForReservation,
                IsSupportedByNotification = true,
                Conditions = s.BiharDomicileForReservation
                    ? "Requires Bihar permanent domicile + valid SC caste certificate."
                    : "Requires valid SC caste certificate."
            });
            exam.AgeRelaxations.Add(new AgeRelaxation
            {
                Category = CategoryType.ST,
                RelaxationYears = s.ScStRelaxation,
                RequiresDomicileForStateQuota = s.BiharDomicileForReservation,
                IsSupportedByNotification = true,
                Conditions = s.BiharDomicileForReservation
                    ? "Requires Bihar permanent domicile + valid ST caste certificate."
                    : "Requires valid ST caste certificate."
            });
            exam.AgeRelaxations.Add(new AgeRelaxation
            {
                Category = CategoryType.PwBD,
                RelaxationYears = s.PwbdRelaxation,
                RequiresDomicileForStateQuota = false,
                IsSupportedByNotification = true,
                Conditions = "Benchmark disability >= 40% as per RPwD Act 2016."
            });
            if (s.BiharDomicileForReservation)
            {
                exam.AgeRelaxations.Add(new AgeRelaxation
                {
                    Category = CategoryType.FemaleReservation,
                    RelaxationYears = 3,
                    RequiresDomicileForStateQuota = true,
                    IsSupportedByNotification = true,
                    Conditions = "Bihar UR/BC/EBC female upper age limit 40 years (BPSC/Bihar state norms)."
                });
            }
        }

        if (s.Patterns is not null)
        {
            var idx = 1;
            foreach (var p in s.Patterns)
            {
                exam.ExamPatterns.Add(new ExamPattern
                {
                    StageName = p.Stage,
                    SectionName = p.Section,
                    QuestionsCount = p.Qs,
                    MaxMarks = p.Marks,
                    DurationMinutes = p.Mins,
                    NegativeMarkingDetails = p.Neg ?? s.NegativeMarking,
                    IsQualifyingOnly = p.Qual,
                    OrderIndex = idx++
                });
            }
        }

        var sylOrder = 1;
        foreach (var subCode in s.SyllabusSubjectCodes)
        {
            if (subjects.TryGetValue(subCode, out var subj))
            {
                exam.ExamSyllabi.Add(new ExamSyllabus
                {
                    SyllabusSubjectId = subj.Id,
                    StageName = "Core Syllabus",
                    WeightageNotes = "Mapped from official recruitment syllabus.",
                    OfficialSyllabusReferenceUrl = source.OfficialSourceUrl,
                    DisplayOrder = sylOrder++
                });
            }
        }

        if (s.History is not null)
        {
            foreach (var h in s.History)
            {
                exam.HistoricalRecruitments.Add(new HistoricalRecruitment
                {
                    CycleYear = h.Year,
                    DataStatus = h.Status,
                    NotificationDate = h.NotifDate,
                    PrelimExamDate = h.ExamDate,
                    TotalVacancies = h.Total,
                    ItVacancies = h.It,
                    GeneralVacancies = h.Total.HasValue ? Math.Max(0, h.Total.Value - (h.It ?? (s.IsIt ? h.Total.Value : 0))) : null,
                    SelectionPatternSummary = s.SelectionProcedure,
                    OfficialSourceUrl = source.OfficialSourceUrl,
                    Notes = h.Notes
                });
            }
        }

        if (s.Cutoffs is not null)
        {
            foreach (var c in s.Cutoffs)
            {
                exam.PreviousYearCutoffs.Add(new PreviousYearCutoff
                {
                    CycleYear = c.Year,
                    StageName = c.Stage,
                    Category = c.Cat,
                    CutoffMarks = c.Marks,
                    TotalMaxMarks = c.Max,
                    SourceUrl = source.OfficialSourceUrl
                });
            }
        }

        exam.PreviousYearPapers.Add(new PreviousYearPaper
        {
            CycleYear = s.CycleYear > 2025 ? 2025 : s.CycleYear,
            StageName = "Official Archive",
            Title = $"{s.ShortName} Official Question Paper / Syllabus Archive",
            PaperUrl = org.OfficialWebsiteUrl,
            SourceLabel = $"{org.ShortCode} Official Website",
            IsOfficial = true
        });

        return exam;
    }
}
