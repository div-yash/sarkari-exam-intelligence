using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class ExamNotification : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int CycleYear { get; set; }
    public string? NotificationNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public ExamStatus Status { get; set; } = ExamStatus.UpcomingNotification;
    public string? StatusReason { get; set; }
    public bool IsCurrentCycle { get; set; } = true;

    // Separate confirmed and expected dates — never invent missing dates
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
    public bool IsPrelimDateExpected { get; set; }

    public DateOnly? MainsExamDate { get; set; }
    public DateOnly? ExpectedMainsExamDate { get; set; }
    public bool IsMainsDateExpected { get; set; }

    public DateOnly? InterviewDate { get; set; }
    public DateOnly? ExpectedInterviewDate { get; set; }
    public bool IsInterviewDateExpected { get; set; }

    public DateOnly? ResultDate { get; set; }
    public DateOnly? ExpectedResultDate { get; set; }
    public bool IsResultDateExpected { get; set; }

    public DateOnly? AgeCutoffDate { get; set; }
    public string? AgeCutoffReferenceText { get; set; }

    // Details & Compensation
    public string ApplicationFeeSummary { get; set; } = "Not Announced";
    public string SalaryPayLevel { get; set; } = "Not Announced";
    public string SelectionProcedure { get; set; } = "Not Announced";
    public ExamMode ExamMode { get; set; } = ExamMode.ComputerBasedTest;
    public string NegativeMarkingDetails { get; set; } = "Not Announced";

    // Traceability & Verification
    public string? OfficialNotificationPdfUrl { get; set; }
    public string? OfficialApplicationUrl { get; set; }
    public DateOnly? SourcePublicationDate { get; set; }
    public DateTime? LastVerifiedAtUtc { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.VerifiedOfficial;
    public DataConfidence DataConfidence { get; set; } = DataConfidence.ConfirmedOfficial;
    public bool IsPendingApproval { get; set; }
    public string? ContentHash { get; set; }
    public string? LastChangeSummary { get; set; }
    public DateTime? LastChangedAtUtc { get; set; }

    public ExamVacancy? Vacancy { get; set; }
    public ICollection<ExamDate> ExamDates { get; set; } = new List<ExamDate>();
    public ICollection<EligibilityRule> EligibilityRules { get; set; } = new List<EligibilityRule>();
    public ICollection<SourceVerificationLog> VerificationLogs { get; set; } = new List<SourceVerificationLog>();
}
