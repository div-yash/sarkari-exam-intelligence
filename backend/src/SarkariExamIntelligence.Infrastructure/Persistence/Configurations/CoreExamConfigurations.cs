using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SarkariExamIntelligence.Domain.Entities;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DomicileState).HasMaxLength(100).IsRequired();
        builder.Property(x => x.GraduationQualification).HasMaxLength(120).IsRequired();
        builder.Property(x => x.EngineeringBranch).HasMaxLength(120);
        builder.Property(x => x.University).HasMaxLength(200);
        builder.Property(x => x.GraduationPercentage).HasPrecision(5, 2);
        builder.Property(x => x.GraduationCgpa).HasPrecision(4, 2);
        builder.Property(x => x.Nationality).HasMaxLength(80).IsRequired();
        builder.Property(x => x.GateSubject).HasMaxLength(80);
        builder.Property(x => x.CurrentOrganizationType).HasMaxLength(100);
        builder.Property(x => x.AdditionalQualifications).HasMaxLength(500);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(600);
    }
}

public class ExamOrganizationConfiguration : IEntityTypeConfiguration<ExamOrganization>
{
    public void Configure(EntityTypeBuilder<ExamOrganization> builder)
    {
        builder.ToTable("ExamOrganizations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ShortCode).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ShortCode).HasMaxLength(40).IsRequired();
        builder.Property(x => x.OrganizationType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StateName).HasMaxLength(100);
        builder.Property(x => x.OfficialWebsiteUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.RecruitmentPortalUrl).HasMaxLength(500);
    }
}

public class OfficialSourceConfiguration : IEntityTypeConfiguration<OfficialSource>
{
    public void Configure(EntityTypeBuilder<OfficialSource> builder)
    {
        builder.ToTable("OfficialSources");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.OfficialSourceUrl);
        builder.Property(x => x.SourceName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.OfficialSourceUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.CalendarOrFeedUrl).HasMaxLength(500);
        builder.Property(x => x.LastNotificationPdfUrl).HasMaxLength(600);
        builder.Property(x => x.LastContentHash).HasMaxLength(128);

        builder.HasOne(x => x.Organization)
            .WithMany(o => o.OfficialSources)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SourceVerificationLogConfiguration : IEntityTypeConfiguration<SourceVerificationLog>
{
    public void Configure(EntityTypeBuilder<SourceVerificationLog> builder)
    {
        builder.ToTable("SourceVerificationLogs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CheckedAtUtc);
        builder.Property(x => x.CheckMethod).HasMaxLength(60).IsRequired();
        builder.Property(x => x.ContentHash).HasMaxLength(128);
        builder.Property(x => x.VerifiedByUserId).HasMaxLength(100);

        builder.HasOne(x => x.OfficialSource)
            .WithMany(s => s.VerificationLogs)
            .HasForeignKey(x => x.OfficialSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ExamNotification)
            .WithMany(n => n.VerificationLogs)
            .HasForeignKey(x => x.ExamNotificationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exams");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => new { x.CategoryId, x.IsApprovedForPublish });
        builder.HasIndex(x => new { x.IsBiharSpecific, x.IsItSpecific, x.IsGraduateLevel });

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.ShortName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Department).HasMaxLength(250).IsRequired();
        builder.Property(x => x.JobDesignation).HasMaxLength(250).IsRequired();
        builder.Property(x => x.OfficialRecruitmentWebsite).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Summary).HasMaxLength(2000);
        builder.Property(x => x.DefaultSalaryPayLevel).HasMaxLength(250);
        builder.Property(x => x.DefaultSelectionProcedure).HasMaxLength(1000);
        builder.Property(x => x.DefaultNegativeMarking).HasMaxLength(300);

        builder.HasOne(x => x.Category)
            .WithMany(c => c.Exams)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Organization)
            .WithMany(o => o.Exams)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OfficialSource)
            .WithMany(s => s.Exams)
            .HasForeignKey(x => x.OfficialSourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ExamNotificationConfiguration : IEntityTypeConfiguration<ExamNotification>
{
    public void Configure(EntityTypeBuilder<ExamNotification> builder)
    {
        builder.ToTable("ExamNotifications");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.ExamId, x.CycleYear, x.IsCurrentCycle });
        builder.HasIndex(x => new { x.Status, x.ApplicationEndDate });
        builder.HasIndex(x => x.NotificationDate);

        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.NotificationNumber).HasMaxLength(150);
        builder.Property(x => x.StatusReason).HasMaxLength(500);
        builder.Property(x => x.AgeCutoffReferenceText).HasMaxLength(300);
        builder.Property(x => x.ApplicationFeeSummary).HasMaxLength(500);
        builder.Property(x => x.SalaryPayLevel).HasMaxLength(300);
        builder.Property(x => x.SelectionProcedure).HasMaxLength(1200);
        builder.Property(x => x.NegativeMarkingDetails).HasMaxLength(400);
        builder.Property(x => x.OfficialNotificationPdfUrl).HasMaxLength(600);
        builder.Property(x => x.OfficialApplicationUrl).HasMaxLength(600);
        builder.Property(x => x.ContentHash).HasMaxLength(128);
        builder.Property(x => x.LastChangeSummary).HasMaxLength(1000);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.Notifications)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ExamDateConfiguration : IEntityTypeConfiguration<ExamDate>
{
    public void Configure(EntityTypeBuilder<ExamDate> builder)
    {
        builder.ToTable("ExamDates");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.ExamNotificationId, x.EventType });
        builder.HasIndex(x => x.ConfirmedDate);
        builder.HasIndex(x => x.ExpectedDate);
        builder.Property(x => x.Label).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DisplayStatus).HasMaxLength(60).IsRequired();

        builder.HasOne(x => x.ExamNotification)
            .WithMany(n => n.ExamDates)
            .HasForeignKey(x => x.ExamNotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EligibilityRuleConfiguration : IEntityTypeConfiguration<EligibilityRule>
{
    public void Configure(EntityTypeBuilder<EligibilityRule> builder)
    {
        builder.ToTable("EligibilityRules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ExamId);

        builder.Property(x => x.MinimumQualificationLevel).HasMaxLength(100).IsRequired();
        builder.Property(x => x.AcceptedEngineeringBranchesCsv).HasMaxLength(1000);
        builder.Property(x => x.QualificationSummary).HasMaxLength(1500).IsRequired();
        builder.Property(x => x.MinPercentageGeneral).HasPrecision(5, 2);
        builder.Property(x => x.MinPercentageReserved).HasPrecision(5, 2);
        builder.Property(x => x.MinCgpa).HasPrecision(4, 2);
        builder.Property(x => x.NationalityRequirement).HasMaxLength(250);
        builder.Property(x => x.DomicileRequirement).HasMaxLength(300);
        builder.Property(x => x.BiharCertificateConditions).HasMaxLength(800);
        builder.Property(x => x.GenderRequirement).HasMaxLength(60);
        builder.Property(x => x.AcceptedGatePapersCsv).HasMaxLength(200);
        builder.Property(x => x.SourceReference).HasMaxLength(600);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.EligibilityRules)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ExamNotification)
            .WithMany(n => n.EligibilityRules)
            .HasForeignKey(x => x.ExamNotificationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class EducationalQualificationConfiguration : IEntityTypeConfiguration<EducationalQualification>
{
    public void Configure(EntityTypeBuilder<EducationalQualification> builder)
    {
        builder.ToTable("EducationalQualifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DegreeLevel).HasMaxLength(80).IsRequired();
        builder.Property(x => x.DegreeName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.AcceptedBranchesCsv).HasMaxLength(1000);
        builder.Property(x => x.MinimumPercentage).HasPrecision(5, 2);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.EducationalQualifications)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AgeRelaxationConfiguration : IEntityTypeConfiguration<AgeRelaxation>
{
    public void Configure(EntityTypeBuilder<AgeRelaxation> builder)
    {
        builder.ToTable("AgeRelaxations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.ExamId, x.Category });
        builder.Property(x => x.SubCategory).HasMaxLength(120);
        builder.Property(x => x.Conditions).HasMaxLength(500).IsRequired();
        builder.Property(x => x.SourceReference).HasMaxLength(500);

        builder.HasOne(x => x.Exam)
            .WithMany(e => e.AgeRelaxations)
            .HasForeignKey(x => x.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ExamVacancyConfiguration : IEntityTypeConfiguration<ExamVacancy>
{
    public void Configure(EntityTypeBuilder<ExamVacancy> builder)
    {
        builder.ToTable("ExamVacancies");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ExamNotificationId).IsUnique();
        builder.Property(x => x.Notes).HasMaxLength(600);
        builder.Property(x => x.SourceReference).HasMaxLength(600);

        builder.HasOne(x => x.ExamNotification)
            .WithOne(n => n.Vacancy)
            .HasForeignKey<ExamVacancy>(x => x.ExamNotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CategoryWiseVacancyConfiguration : IEntityTypeConfiguration<CategoryWiseVacancy>
{
    public void Configure(EntityTypeBuilder<CategoryWiseVacancy> builder)
    {
        builder.ToTable("CategoryWiseVacancies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SubCategory).HasMaxLength(100);

        builder.HasOne(x => x.ExamVacancy)
            .WithMany(v => v.CategoryWiseVacancies)
            .HasForeignKey(x => x.ExamVacancyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
