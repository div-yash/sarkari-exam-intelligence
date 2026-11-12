using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Infrastructure.Identity;

namespace SarkariExamIntelligence.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ExamOrganization> ExamOrganizations => Set<ExamOrganization>();
    public DbSet<OfficialSource> OfficialSources => Set<OfficialSource>();
    public DbSet<SourceVerificationLog> SourceVerificationLogs => Set<SourceVerificationLog>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamNotification> ExamNotifications => Set<ExamNotification>();
    public DbSet<ExamDate> ExamDates => Set<ExamDate>();
    public DbSet<EligibilityRule> EligibilityRules => Set<EligibilityRule>();
    public DbSet<EducationalQualification> EducationalQualifications => Set<EducationalQualification>();
    public DbSet<AgeRelaxation> AgeRelaxations => Set<AgeRelaxation>();
    public DbSet<ExamVacancy> ExamVacancies => Set<ExamVacancy>();
    public DbSet<CategoryWiseVacancy> CategoryWiseVacancies => Set<CategoryWiseVacancy>();
    public DbSet<SyllabusSubject> SyllabusSubjects => Set<SyllabusSubject>();
    public DbSet<SyllabusTopic> SyllabusTopics => Set<SyllabusTopic>();
    public DbSet<ExamSyllabus> ExamSyllabi => Set<ExamSyllabus>();
    public DbSet<ExamPattern> ExamPatterns => Set<ExamPattern>();
    public DbSet<HistoricalRecruitment> HistoricalRecruitments => Set<HistoricalRecruitment>();
    public DbSet<HistoricalCategoryVacancy> HistoricalCategoryVacancies => Set<HistoricalCategoryVacancy>();
    public DbSet<PreviousYearCutoff> PreviousYearCutoffs => Set<PreviousYearCutoff>();
    public DbSet<PreviousYearPaper> PreviousYearPapers => Set<PreviousYearPaper>();
    public DbSet<SavedExam> SavedExams => Set<SavedExam>();
    public DbSet<UserApplication> UserApplications => Set<UserApplication>();
    public DbSet<UserStudyPlan> UserStudyPlans => Set<UserStudyPlan>();
    public DbSet<UserStudyProgress> UserStudyProgresses => Set<UserStudyProgress>();
    public DbSet<MockTestResult> MockTestResults => Set<MockTestResult>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var user = _currentUserService?.Email ?? _currentUserService?.UserId?.ToString() ?? "system";

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAtUtc == default)
                    entry.Entity.CreatedAtUtc = now;
                entry.Entity.CreatedBy ??= user;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
                entry.Entity.UpdatedBy = user;
            }
        }

        // Ensure PostgreSQL timestamptz compatibility by normalizing any Unspecified DateTime Kind to UTC
        foreach (var entry in ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            foreach (var prop in entry.Properties)
            {
                if (prop.CurrentValue is DateTime dt && dt.Kind == DateTimeKind.Unspecified)
                {
                    prop.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
