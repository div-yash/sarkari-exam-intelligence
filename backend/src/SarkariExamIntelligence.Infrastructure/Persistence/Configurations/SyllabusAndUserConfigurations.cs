using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SarkariExamIntelligence.Domain.Entities;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Configurations;

public class SyllabusSubjectConfiguration : IEntityTypeConfiguration<SyllabusSubject>
{
    public void Configure(EntityTypeBuilder<SyllabusSubject> builder)
    {
        builder.ToTable("SyllabusSubjects");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Code).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(800);
    }
}

public class SyllabusTopicConfiguration : IEntityTypeConfiguration<SyllabusTopic>
{
    public void Configure(EntityTypeBuilder<SyllabusTopic> builder)
    {
        builder.ToTable("SyllabusTopics");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.SyllabusSubjectId, x.DisplayOrder });
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(800);
        builder.Property(x => x.DifficultyLevel).HasMaxLength(40).IsRequired();

        builder.HasOne(x => x.SyllabusSubject)
            .WithMany(s => s.Topics)
            .HasForeignKey(x => x.SyllabusSubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ExamSyllabusConfiguration : IEntityTypeConfiguration<ExamSyllabus>
{
    public void Configure(EntityTypeBuilder<ExamSyllabus> builder)
    {
        builder.ToTable("ExamSyllabi");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.ExamId, x.SyllabusSubjectId, x.StageName }).IsUnique();
        builder.Property(x => x.StageName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.WeightageNotes).HasMaxLength(400);
        builder.Property(x => x.ExamSpecificTopicsNotes).HasMaxLength(1500);
        builder.Property(x => x.OfficialSyllabusReferenceUrl).HasMaxLength(600);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.ExamSyllabi)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SyllabusSubject)
            .WithMany(s => s.ExamSyllabi)
            .HasForeignKey(x => x.SyllabusSubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ExamPatternConfiguration : IEntityTypeConfiguration<ExamPattern>
{
    public void Configure(EntityTypeBuilder<ExamPattern> builder)
    {
        builder.ToTable("ExamPatterns");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.ExamId, x.OrderIndex });
        builder.Property(x => x.StageName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.SectionName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.MaxMarks).HasPrecision(7, 2);
        builder.Property(x => x.NegativeMarkingDetails).HasMaxLength(250);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.ExamPatterns)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class HistoricalRecruitmentConfiguration : IEntityTypeConfiguration<HistoricalRecruitment>
{
    public void Configure(EntityTypeBuilder<HistoricalRecruitment> builder)
    {
        builder.ToTable("HistoricalRecruitments");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.ExamId, x.CycleYear }).IsUnique();
        builder.Property(x => x.SelectionPatternSummary).HasMaxLength(800);
        builder.Property(x => x.OfficialSourceUrl).HasMaxLength(600);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.HistoricalRecruitments)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class HistoricalCategoryVacancyConfiguration : IEntityTypeConfiguration<HistoricalCategoryVacancy>
{
    public void Configure(EntityTypeBuilder<HistoricalCategoryVacancy> builder)
    {
        builder.ToTable("HistoricalCategoryVacancies");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.HistoricalRecruitment)
            .WithMany(h => h.CategoryVacancies)
            .HasForeignKey(x => x.HistoricalRecruitmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PreviousYearCutoffConfiguration : IEntityTypeConfiguration<PreviousYearCutoff>
{
    public void Configure(EntityTypeBuilder<PreviousYearCutoff> builder)
    {
        builder.ToTable("PreviousYearCutoffs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.ExamId, x.CycleYear });
        builder.Property(x => x.StageName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.CutoffMarks).HasPrecision(7, 2);
        builder.Property(x => x.TotalMaxMarks).HasPrecision(7, 2);
        builder.Property(x => x.SourceUrl).HasMaxLength(600);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.PreviousYearCutoffs)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.HistoricalRecruitment)
            .WithMany(h => h.Cutoffs)
            .HasForeignKey(x => x.HistoricalRecruitmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class PreviousYearPaperConfiguration : IEntityTypeConfiguration<PreviousYearPaper>
{
    public void Configure(EntityTypeBuilder<PreviousYearPaper> builder)
    {
        builder.ToTable("PreviousYearPapers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StageName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.PaperUrl).HasMaxLength(600).IsRequired();
        builder.Property(x => x.SourceLabel).HasMaxLength(150).IsRequired();

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.PreviousYearPapers)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SavedExamConfiguration : IEntityTypeConfiguration<SavedExam>
{
    public void Configure(EntityTypeBuilder<SavedExam> builder)
    {
        builder.ToTable("SavedExams");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.ExamId }).IsUnique();
        builder.Property(x => x.PriorityLevel).HasMaxLength(30).IsRequired();

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.SavedByUsers)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserApplicationConfiguration : IEntityTypeConfiguration<UserApplication>
{
    public void Configure(EntityTypeBuilder<UserApplication> builder)
    {
        builder.ToTable("UserApplications");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.ExamId }).IsUnique();
        builder.Property(x => x.ApplicationReferenceNumber).HasMaxLength(120);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.UserApplications)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ExamNotification)
            .WithMany()
            .HasForeignKey(x => x.ExamNotificationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class UserStudyPlanConfiguration : IEntityTypeConfiguration<UserStudyPlan>
{
    public void Configure(EntityTypeBuilder<UserStudyPlan> builder)
    {
        builder.ToTable("UserStudyPlans");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.ExamId });
        builder.Property(x => x.PlanTitle).HasMaxLength(200).IsRequired();

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.UserStudyPlans)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserStudyProgressConfiguration : IEntityTypeConfiguration<UserStudyProgress>
{
    public void Configure(EntityTypeBuilder<UserStudyProgress> builder)
    {
        builder.ToTable("UserStudyProgresses");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.SyllabusTopicId, x.ExamId }).IsUnique();

        builder.HasOne(x => x.SyllabusTopic)
            .WithMany(t => t.UserProgresses)
            .HasForeignKey(x => x.SyllabusTopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Exam)
            .WithMany()
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class MockTestResultConfiguration : IEntityTypeConfiguration<MockTestResult>
{
    public void Configure(EntityTypeBuilder<MockTestResult> builder)
    {
        builder.ToTable("MockTestResults");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.ExamId, x.TestDate });
        builder.Property(x => x.TestTitle).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ScoreObtained).HasPrecision(7, 2);
        builder.Property(x => x.MaxScore).HasPrecision(7, 2);
        builder.Property(x => x.AccuracyPercent).HasPrecision(5, 2);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.MockTestResults)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.DeduplicationKey }).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.IsRead, x.ScheduledForUtc });
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(1500).IsRequired();
        builder.Property(x => x.DeduplicationKey).HasMaxLength(220).IsRequired();

        builder.HasOne(x => x.Exam)
            .WithMany()
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ExamNotification)
            .WithMany()
            .HasForeignKey(x => x.ExamNotificationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.OverrideEmail).HasMaxLength(256);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TimestampUtc);
        builder.HasIndex(x => new { x.EntityName, x.EntityId });
        builder.Property(x => x.EntityName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Action).HasMaxLength(80).IsRequired();
        builder.Property(x => x.UserEmail).HasMaxLength(256);
    }
}
