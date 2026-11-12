using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Domain.Entities;

namespace SarkariExamIntelligence.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<Category> Categories { get; }
    DbSet<ExamOrganization> ExamOrganizations { get; }
    DbSet<OfficialSource> OfficialSources { get; }
    DbSet<SourceVerificationLog> SourceVerificationLogs { get; }
    DbSet<Exam> Exams { get; }
    DbSet<ExamNotification> ExamNotifications { get; }
    DbSet<ExamDate> ExamDates { get; }
    DbSet<EligibilityRule> EligibilityRules { get; }
    DbSet<EducationalQualification> EducationalQualifications { get; }
    DbSet<AgeRelaxation> AgeRelaxations { get; }
    DbSet<ExamVacancy> ExamVacancies { get; }
    DbSet<CategoryWiseVacancy> CategoryWiseVacancies { get; }
    DbSet<SyllabusSubject> SyllabusSubjects { get; }
    DbSet<SyllabusTopic> SyllabusTopics { get; }
    DbSet<ExamSyllabus> ExamSyllabi { get; }
    DbSet<ExamPattern> ExamPatterns { get; }
    DbSet<HistoricalRecruitment> HistoricalRecruitments { get; }
    DbSet<HistoricalCategoryVacancy> HistoricalCategoryVacancies { get; }
    DbSet<PreviousYearCutoff> PreviousYearCutoffs { get; }
    DbSet<PreviousYearPaper> PreviousYearPapers { get; }
    DbSet<SavedExam> SavedExams { get; }
    DbSet<UserApplication> UserApplications { get; }
    DbSet<UserStudyPlan> UserStudyPlans { get; }
    DbSet<UserStudyProgress> UserStudyProgresses { get; }
    DbSet<MockTestResult> MockTestResults { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
