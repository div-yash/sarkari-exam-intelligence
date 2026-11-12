namespace SarkariExamIntelligence.Domain.Enums;

public enum ExamStatus
{
    UpcomingNotification = 1,
    NotificationReleased = 2,
    ApplicationsOpen = 3,
    ClosingSoon = 4,
    ApplicationsClosed = 5,
    AdmitCardReleased = 6,
    ExaminationScheduled = 7,
    ExaminationCompleted = 8,
    ResultDeclared = 9,
    Postponed = 10,
    Cancelled = 11
}

public enum VerificationStatus
{
    PendingApproval = 1,
    VerifiedOfficial = 2,
    TentativeCalendar = 3,
    NeedsReverification = 4,
    ThirdPartyDiscovered = 5,
    Rejected = 6
}

public enum DataConfidence
{
    ConfirmedOfficial = 1,
    OfficialCalendarTentative = 2,
    HistoricalCycleExpected = 3,
    UnverifiedDraft = 4,
    NotAnnounced = 5
}

public enum EligibilityResultStatus
{
    Eligible = 1,
    PotentiallyEligibleVerificationRequired = 2,
    NotEligible = 3
}

public enum StudyProgressStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3
}

public enum GroupClassification
{
    GroupA = 1,
    GroupB = 2,
    GroupBGazetted = 3,
    GroupBNonGazetted = 4,
    GroupC = 5,
    NotSpecified = 6
}

public enum JobNature
{
    Permanent = 1,
    Contractual = 2,
    FixedTermRenewable = 3,
    FellowshipOrScore = 4
}

public enum ExamMode
{
    ComputerBasedTest = 1,
    PenAndPaperOMR = 2,
    DescriptiveWritten = 3,
    CombinedCBTAndDescriptive = 4,
    GateScorePlusInterview = 5,
    PhysicalAndWritten = 6,
    NotSpecified = 7
}

public enum VacancyType
{
    Official = 1,
    Proposed = 2,
    HistoricalReference = 3,
    NotAnnounced = 4
}

public enum HistoricalDataStatus
{
    Verified = 1,
    AwaitingOfficialNotification = 2,
    Unavailable = 3
}

public enum CategoryType
{
    General = 1,
    EWS = 2,
    OBCNCL = 3,
    SC = 4,
    ST = 5,
    PwBD = 6,
    BiharEBC = 7,
    BiharBC = 8,
    BiharWBC = 9,
    FemaleReservation = 10,
    ExServicemen = 11
}

public enum GenderType
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum CalendarEventType
{
    NotificationRelease = 1,
    ApplicationOpen = 2,
    ApplicationDeadline = 3,
    CorrectionWindow = 4,
    AdmitCardRelease = 5,
    PrelimsExam = 6,
    MainsExam = 7,
    Interview = 8,
    ResultDeclaration = 9
}

public enum SubjectGroup
{
    Common = 1,
    Technical = 2
}

public enum ApplicationTrackingStatus
{
    Interested = 1,
    Preparing = 2,
    Applied = 3,
    AdmitCardDownloaded = 4,
    Appeared = 5,
    QualifiedStage = 6,
    NotSelected = 7
}

public enum NotificationType
{
    NewOfficialNotification = 1,
    ApplicationsOpened = 2,
    Deadline30Days = 3,
    Deadline7Days = 4,
    Deadline3Days = 5,
    Deadline1Day = 6,
    ExamDateChanged = 7,
    AdmitCardReleased = 8,
    ResultDeclared = 9,
    SourceRecordUpdated = 10
}

public enum NotificationChannel
{
    InApp = 1,
    Email = 2,
    Both = 3
}

public enum VerificationOutcome
{
    ConfirmedUnchanged = 1,
    ChangesDetected = 2,
    TentativeMatch = 3,
    SourceUnreachable = 4,
    ManualVerificationRequired = 5
}
