export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export type ExamStatus =
  | 'UpcomingNotification'
  | 'NotificationReleased'
  | 'ApplicationsOpen'
  | 'ClosingSoon'
  | 'ApplicationsClosed'
  | 'AdmitCardReleased'
  | 'ExaminationScheduled'
  | 'ExaminationCompleted'
  | 'ResultDeclared'
  | 'Postponed'
  | 'Cancelled';

export type EligibilityResultStatus =
  | 'Eligible'
  | 'PotentiallyEligibleVerificationRequired'
  | 'NotEligible';

export type VerificationStatus =
  | 'PendingApproval'
  | 'VerifiedOfficial'
  | 'TentativeCalendar'
  | 'NeedsReverification'
  | 'ThirdPartyDiscovered'
  | 'Rejected';

export type DataConfidence =
  | 'ConfirmedOfficial'
  | 'OfficialCalendarTentative'
  | 'HistoricalCycleExpected'
  | 'UnverifiedDraft'
  | 'NotAnnounced';

export type StudyProgressStatus = 'NotStarted' | 'InProgress' | 'Completed';

export type CategoryType =
  | 'General'
  | 'EWS'
  | 'OBCNCL'
  | 'SC'
  | 'ST'
  | 'PwBD'
  | 'BiharEBC'
  | 'BiharBC'
  | 'BiharWBC'
  | 'FemaleReservation'
  | 'ExServicemen';

export interface UserProfile {
  id: string;
  userId: string;
  email: string;
  fullName: string;
  dateOfBirth?: string;
  gender?: string;
  category: CategoryType;
  isPwBD: boolean;
  isExServiceman: boolean;
  domicileState: string;
  graduationQualification: string;
  engineeringBranch?: string;
  university?: string;
  graduationPercentage?: number;
  graduationCgpa?: number;
  passingYear?: number;
  workExperienceMonths: number;
  isWorkingProfessional: boolean;
  currentOrganizationType?: string;
  additionalQualifications?: string;
  hasValidGateScore: boolean;
  gateSubject?: string;
  gateScore?: number;
  gateYear?: number;
  hasBiharDomicileCertificate: boolean;
  hasValidNclOrEwsCertificate: boolean;
  nationality: string;
  profileCompletenessPercent: number;
}

export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  userId: string;
  email: string;
  fullName: string;
  roles: string[];
  profile?: UserProfile;
}

export interface CategorySummary {
  id: string;
  name: string;
  slug: string;
  code: string;
  description: string;
  examCount: number;
}

export interface ExamListItem {
  id: string;
  currentNotificationId?: string;
  name: string;
  shortName: string;
  slug: string;
  code: string;
  categoryName: string;
  categorySlug: string;
  organizationName: string;
  organizationCode: string;
  department: string;
  jobDesignation: string;
  groupClassification: string;
  jobNature: string;
  isBiharSpecific: boolean;
  isItSpecific: boolean;
  isGraduateLevel: boolean;
  cycleYear?: number;
  status: ExamStatus;
  notificationDate?: string;
  expectedNotificationDate?: string;
  isNotificationDateExpected: boolean;
  applicationStartDate?: string;
  expectedApplicationStartDate?: string;
  isApplicationStartExpected: boolean;
  applicationEndDate?: string;
  expectedApplicationEndDate?: string;
  isApplicationEndExpected: boolean;
  examDate?: string;
  expectedExamDate?: string;
  isExamDateExpected: boolean;
  daysToDeadline?: number;
  totalVacancies?: number;
  itSpecificVacancies?: number;
  vacancyType: string;
  minAge?: number;
  maxAge?: number;
  qualificationSummary: string;
  verificationStatus: VerificationStatus;
  dataConfidence: DataConfidence;
  officialRecruitmentWebsite: string;
  officialNotificationPdfUrl?: string;
  officialApplicationUrl?: string;
  isSaved: boolean;
  userEligibilityStatus?: EligibilityResultStatus;
  userEligibilitySummary?: string;
}

export interface DateField {
  confirmedDate?: string;
  expectedDate?: string;
  isExpected: boolean;
  displayText: string;
  dateLabelType: 'Confirmed' | 'Expected' | 'Not Announced';
}

export interface ExamDetail {
  id: string;
  currentNotificationId?: string;
  name: string;
  shortName: string;
  slug: string;
  code: string;
  categoryName: string;
  categorySlug: string;
  conductingOrganization: string;
  organizationShortCode: string;
  department: string;
  jobDesignation: string;
  groupClassification: string;
  jobNature: string;
  summary: string;
  isBiharSpecific: boolean;
  isItSpecific: boolean;
  isGraduateLevel: boolean;
  cycleYear?: number;
  notificationNumber?: string;
  status: ExamStatus;
  statusReason?: string;
  notificationPublicationDate: DateField;
  applicationOpeningDate: DateField;
  applicationClosingDate: DateField;
  correctionWindowStart?: string;
  correctionWindowEnd?: string;
  admitCardDate: DateField;
  preliminaryExamDate: DateField;
  mainsExamDate: DateField;
  interviewDate: DateField;
  resultDate: DateField;
  eligibilityRule?: {
    minAgeYears: number;
    maxAgeYears?: number;
    ageCutoffDate?: string;
    ageCutoffBasisDescription: string;
    minimumQualificationLevel: string;
    requiresEngineeringOrTechDegree: boolean;
    acceptsAnyGraduateDegree: boolean;
    acceptedEngineeringBranches?: string;
    qualificationSummary: string;
    minPercentageGeneral?: number;
    minPercentageReserved?: number;
    minCgpa?: number;
    finalYearAppearingEligible: boolean;
    requiresWorkExperience: boolean;
    minWorkExperienceMonths: number;
    experienceRequirementNotes?: string;
    nationalityRequirement: string;
    domicileRequirement?: string;
    biharDomicileRequiredForReservation: boolean;
    biharCertificateConditions?: string;
    genderRequirement?: string;
    requiresValidGateScore: boolean;
    acceptedGatePapers?: string;
    minGateScore?: number;
    additionalConditions?: string;
    verificationNotes?: string;
    sourceReference?: string;
  };
  educationalQualifications: Array<{
    degreeLevel: string;
    degreeName: string;
    acceptedBranches?: string;
    minimumPercentage?: number;
    isMandatory: boolean;
    notes?: string;
  }>;
  ageRelaxations: Array<{
    category: string;
    subCategory?: string;
    relaxationYears: number;
    maxUpperAgeOverride?: number;
    requiresDomicileForStateQuota: boolean;
    isSupportedByNotification: boolean;
    conditions: string;
  }>;
  vacancy?: {
    totalVacancies?: number;
    itSpecificVacancies?: number;
    generalVacancies?: number;
    vacancyType: string;
    notes?: string;
    sourceReference?: string;
    categoryWiseVacancies: Array<{
      category: string;
      subCategory?: string;
      vacancyCount?: number;
      notes?: string;
    }>;
  };
  applicationFee: string;
  salaryPayLevel: string;
  selectionProcedure: string;
  examMode: string;
  negativeMarking: string;
  examPatterns: Array<{
    stageName: string;
    sectionName: string;
    questionsCount?: number;
    maxMarks?: number;
    durationMinutes?: number;
    negativeMarkingDetails?: string;
    isQualifyingOnly: boolean;
  }>;
  syllabi: Array<{
    subjectId: string;
    subjectCode: string;
    subjectName: string;
    subjectGroup: string;
    stageName: string;
    weightageNotes?: string;
    examSpecificTopicsNotes?: string;
    officialSyllabusReferenceUrl?: string;
    topics: string[];
  }>;
  historicalRecruitments: Array<{
    cycleYear: number;
    dataStatus: string;
    notificationDate?: string;
    applicationStartDate?: string;
    applicationEndDate?: string;
    prelimExamDate?: string;
    mainsExamDate?: string;
    resultDate?: string;
    totalVacancies?: number;
    itVacancies?: number;
    generalVacancies?: number;
    selectionPatternSummary?: string;
    officialSourceUrl?: string;
    notes?: string;
  }>;
  previousYearCutoffs: Array<{
    cycleYear: number;
    stageName: string;
    category: string;
    cutoffMarks?: number;
    totalMaxMarks?: number;
    sourceUrl?: string;
    notes?: string;
  }>;
  previousYearPapers: Array<{
    cycleYear: number;
    stageName: string;
    title: string;
    paperUrl: string;
    sourceLabel: string;
    isOfficial: boolean;
  }>;
  officialNotificationPdfUrl?: string;
  officialApplicationUrl?: string;
  officialRecruitmentWebsite: string;
  officialSource: {
    sourceOrganization: string;
    officialSourceUrl: string;
    notificationPdfUrl?: string;
    dateLastCheckedUtc?: string;
    dateLastUpdatedUtc?: string;
    sourcePublicationDate?: string;
    verificationStatus: VerificationStatus;
    dataConfidence: DataConfidence;
    lastChangeSummary?: string;
  };
  isSaved: boolean;
  userApplicationStatus?: string;
}

export interface EligibilityCheckResult {
  examId: string;
  examName: string;
  examSlug: string;
  categoryName: string;
  organizationCode: string;
  currentStatus: ExamStatus;
  applicationEndDate?: string;
  isBiharSpecific: boolean;
  isItSpecific: boolean;
  status: EligibilityResultStatus;
  statusLabel: string;
  summary: string;
  ageEvaluation: {
    candidateDob?: string;
    cutoffDateUsed?: string;
    isCutoffDateTentative: boolean;
    cutoffDateBasis: string;
    calculatedAgeText: string;
    calculatedAgeYearsDecimal?: number;
    baseMinAge: number;
    baseMaxAge?: number;
    appliedRelaxationYears: number;
    effectiveMaxAge?: number;
    relaxationExplanation: string;
    passesAgeRule: boolean;
  };
  criteriaChecks: Array<{
    criterion: string;
    passed: boolean;
    requiresVerification: boolean;
    detail: string;
  }>;
  reasons: string[];
  verificationActionsRequired: string[];
}

export interface EligibilitySummary {
  totalExamsEvaluated: number;
  eligibleCount: number;
  potentiallyEligibleCount: number;
  notEligibleCount: number;
  results: EligibilityCheckResult[];
}

export interface DashboardSummary {
  totalExamsTracked: number;
  currentlyOpenApplications: number;
  upcomingNotificationsCount: number;
  applicationsClosingIn7Days: number;
  applicationsClosingIn30Days: number;
  upcomingExamDatesCount: number;
  examsMatchingMyQualification: number;
  savedExamsCount: number;
  newlyReleasedNotificationsCount: number;
  totalVacanciesAnnounced: number;
  myPreparationProgressPercent: number;
  urgentDeadlines: Array<{
    examId: string;
    examName: string;
    slug: string;
    organizationCode: string;
    applicationEndDate: string;
    daysRemaining: number;
    urgencyLevel: string;
    officialApplicationUrl?: string;
    eligibilityStatus?: EligibilityResultStatus;
  }>;
  openApplicationsList: Array<{
    examId: string;
    name: string;
    slug: string;
    categoryName: string;
    organizationCode: string;
    status: ExamStatus;
    relevantDate?: string;
    isExpected: boolean;
    totalVacancies?: number;
    eligibilityStatus?: EligibilityResultStatus;
  }>;
  upcomingExamDatesList: Array<{
    examId: string;
    name: string;
    slug: string;
    categoryName: string;
    organizationCode: string;
    status: ExamStatus;
    relevantDate?: string;
    isExpected: boolean;
    totalVacancies?: number;
    eligibilityStatus?: EligibilityResultStatus;
  }>;
  examsByCategory: Array<{
    categoryName: string;
    categorySlug: string;
    examCount: number;
    openCount: number;
  }>;
  statusDistribution: Array<{
    statusName: string;
    count: number;
  }>;
}

export interface CalendarEvent {
  examId: string;
  examName: string;
  examSlug: string;
  categoryName: string;
  categorySlug: string;
  organizationCode: string;
  eventType: string;
  title: string;
  eventDate: string;
  isExpected: boolean;
  examStatus: ExamStatus;
}

export interface SyllabusSubject {
  id: string;
  code: string;
  name: string;
  subjectGroup: 'Common' | 'Technical';
  description: string;
  totalTopics: number;
  completedTopics: number;
  inProgressTopics: number;
  topics: SyllabusTopicProgress[];
}

export interface SyllabusTopicProgress {
  topicId: string;
  subjectId: string;
  subjectName: string;
  name: string;
  description: string;
  difficultyLevel: string;
  estimatedHours: number;
  status: StudyProgressStatus;
  revisionCount: number;
  lastRevisedAtUtc?: string;
  preparationNotes?: string;
}

export interface StudyPlan {
  id: string;
  examId: string;
  examName: string;
  examSlug: string;
  planTitle: string;
  dailyTargetMinutes: number;
  weeklyTargetHours: number;
  targetExamDate?: string;
  timetableJson: string;
  isActive: boolean;
}

export interface MockTestResult {
  id: string;
  examId: string;
  examName: string;
  testTitle: string;
  testDate: string;
  scoreObtained: number;
  maxScore: number;
  percentageScore: number;
  accuracyPercent: number;
  timeTakenMinutes: number;
  weakAreasNotes?: string;
}

export interface SavedExam {
  id: string;
  examId: string;
  examName: string;
  examSlug: string;
  categoryName: string;
  organizationCode: string;
  status: ExamStatus;
  applicationEndDate?: string;
  examDate?: string;
  priorityLevel: string;
  personalNotes?: string;
  savedAtUtc: string;
}

export interface UserApplication {
  id: string;
  examId: string;
  examName: string;
  examSlug: string;
  trackingStatus: string;
  appliedDate?: string;
  applicationReferenceNumber?: string;
  documentsChecklistJson: string;
  notes?: string;
  examDate?: string;
  daysToExam?: number;
}

export interface PreparationOverview {
  totalTopicsTracked: number;
  completedTopicsCount: number;
  inProgressTopicsCount: number;
  overallCompletionPercent: number;
  totalRevisions: number;
  activeStudyPlansCount: number;
  mockTestsTaken: number;
  averageMockScorePercent: number;
  studyPlans: StudyPlan[];
  applications: UserApplication[];
  recentMockTests: MockTestResult[];
  savedExams: SavedExam[];
}

export interface VacancyAnalytics {
  disclaimer: string;
  vacanciesByYear: Array<{
    year: number;
    totalAnnouncedVacancies: number;
    itSpecificVacancies: number;
    generalVacancies: number;
    cyclesWithVerifiedData: number;
  }>;
  vacanciesByDepartment: Array<{
    categoryName: string;
    totalVacancies: number;
    itSpecificVacancies: number;
    examCount: number;
  }>;
  vacanciesByExam: Array<{
    examId: string;
    examName: string;
    examSlug: string;
    categoryName: string;
    latestCycleYear: number;
    latestVacancies?: number;
    fiveYearTotalVacancies?: number;
    latestDataStatus: string;
  }>;
  itVsGeneralSplit: {
    itSpecificVacancies: number;
    generalAndOtherVacancies: number;
  };
  categoryWiseVacancies: Array<{
    categoryName: string;
    totalVacancies: number;
  }>;
}

export interface NotificationItem {
  id: string;
  examId?: string;
  examName?: string;
  examSlug?: string;
  notificationType: string;
  title: string;
  message: string;
  channel: string;
  isRead: boolean;
  readAtUtc?: string;
  emailSent: boolean;
  scheduledForUtc: string;
}

export interface NotificationPreference {
  enableInApp: boolean;
  enableEmail: boolean;
  alertOnNewNotification: boolean;
  alertOnApplicationOpen: boolean;
  alert30DaysBefore: boolean;
  alert7DaysBefore: boolean;
  alert3DaysBefore: boolean;
  alert1DayBefore: boolean;
  alertOnExamDateChange: boolean;
  alertOnAdmitCard: boolean;
  alertOnResult: boolean;
  overrideEmail?: string;
}

export interface AdminDashboardOverview {
  totalExams: number;
  pendingApprovalsCount: number;
  unverifiedSourcesCount: number;
  recentChangesCount: number;
  totalUsers: number;
  pendingItems: Array<{
    examId: string;
    examNotificationId?: string;
    examName: string;
    organizationCode: string;
    isExamPendingPublish: boolean;
    isNotificationPendingApproval: boolean;
    verificationStatus: VerificationStatus;
    dataConfidence: DataConfidence;
    lastChangeSummary?: string;
    lastChangedAtUtc?: string;
  }>;
  sources: Array<{
    id: string;
    organizationName: string;
    organizationCode: string;
    sourceName: string;
    officialSourceUrl: string;
    calendarOrFeedUrl?: string;
    dateLastCheckedUtc?: string;
    dateLastUpdatedUtc?: string;
    verificationStatus: VerificationStatus;
    dataConfidence: DataConfidence;
    lastHttpStatusCode?: number;
    isActive: boolean;
    notes?: string;
  }>;
  recentAuditLogs: Array<{
    id: string;
    userEmail?: string;
    entityName: string;
    entityId: string;
    action: string;
    oldValuesJson?: string;
    newValuesJson?: string;
    reason?: string;
    timestampUtc: string;
  }>;
}
