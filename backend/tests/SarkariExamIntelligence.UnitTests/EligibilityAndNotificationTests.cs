using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Services;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;
using SarkariExamIntelligence.Infrastructure.Persistence;
using SarkariExamIntelligence.Infrastructure.Persistence.Seed;

namespace SarkariExamIntelligence.UnitTests;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = new(2026, 11, 11, 12, 0, 0, DateTimeKind.Utc);
    public DateOnly TodayIst { get; set; } = new(2026, 11, 11);
}

public class FakeEmailSender : IEmailSender
{
    public List<(string To, string Subject)> SentEmails { get; } = new();
    public Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        SentEmails.Add((toEmail, subject));
        return Task.CompletedTask;
    }
}

[TestFixture]
public class EligibilityAndNotificationTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public void ExactAgeCalculation_OnNotificationCutoffDate_HandlesBoundaryDays()
    {
        var cutoff = new DateOnly(2026, 8, 1);
        var exact30 = EligibilityEngineService.CalculateExactAge(new DateOnly(1996, 8, 1), cutoff);
        var oneDayOver30 = EligibilityEngineService.CalculateExactAge(new DateOnly(1996, 7, 31), cutoff);

        Assert.That(exact30, Is.EqualTo((30, 0, 0)));
        Assert.That(oneDayOver30, Is.EqualTo((30, 0, 1)));
    }

    [Test]
    public async Task EligibilityEngine_DoesNotAutoMarkBTechITEligible_ForCivilMechElecExam()
    {
        await using var db = CreateInMemoryContext();
        var subjects = await SyllabusLibrarySeeder.SeedAsync(db);
        var refs = await OfficialSourceSeeder.SeedAsync(db);

        var bpscTechSpec = ExamCatalogPart2.GetSpecs().First(s => s.Code == "BPSC-TECH-AE");
        var bpscTech = ExamSeedHelper.BuildExam(bpscTechSpec, refs, subjects);

        var engine = new EligibilityEngineService(db, new FakeDateTimeProvider());
        var btechItCandidate = new UserProfile
        {
            UserId = Guid.NewGuid(),
            FullName = "Bihar IT Grad",
            DateOfBirth = new DateOnly(1999, 6, 15),
            Gender = GenderType.Male,
            Category = CategoryType.OBCNCL,
            DomicileState = "Bihar",
            GraduationQualification = "B.Tech",
            EngineeringBranch = "Information Technology",
            GraduationPercentage = 78.4m,
            HasBiharDomicileCertificate = true,
            HasValidNclOrEwsCertificate = true,
            Nationality = "Indian"
        };

        var result = engine.EvaluateExam(bpscTech, bpscTech.Notifications.First(), btechItCandidate, new DateOnly(2026, 11, 11));
        Assert.That(result.Status, Is.EqualTo(EligibilityResultStatus.NotEligible));
        Assert.That(result.Reasons.Any(r => r.Contains("Information Technology")), Is.True);
    }

    [Test]
    public async Task EligibilityEngine_ValidatesBiharDomicileForStateReservation()
    {
        await using var db = CreateInMemoryContext();
        var subjects = await SyllabusLibrarySeeder.SeedAsync(db);
        var refs = await OfficialSourceSeeder.SeedAsync(db);

        var bpscSpec = ExamCatalogPart2.GetSpecs().First(s => s.Code == "BPSC-CCE");
        var bpscCce = ExamSeedHelper.BuildExam(bpscSpec, refs, subjects);

        var engine = new EligibilityEngineService(db, new FakeDateTimeProvider());
        // Candidate aged 39 (over UR 37, within OBC 40) from Uttar Pradesh -> CANNOT claim Bihar OBC relaxation!
        var upObcCandidate = new UserProfile
        {
            UserId = Guid.NewGuid(),
            FullName = "UP OBC Candidate",
            DateOfBirth = new DateOnly(1987, 1, 1),
            Gender = GenderType.Male,
            Category = CategoryType.OBCNCL,
            DomicileState = "Uttar Pradesh",
            GraduationQualification = "B.Tech",
            EngineeringBranch = "Information Technology",
            GraduationPercentage = 70m,
            Nationality = "Indian"
        };

        var res = engine.EvaluateExam(bpscCce, bpscCce.Notifications.First(), upObcCandidate, new DateOnly(2026, 11, 11));
        Assert.That(res.Status, Is.EqualTo(EligibilityResultStatus.NotEligible));
        Assert.That(res.AgeEvaluation.AppliedRelaxationYears, Is.EqualTo(0));
    }

    [Test]
    public async Task NotificationOrchestrator_DeduplicatesAlertsAcrossRepeatedRuns()
    {
        await using var db = CreateInMemoryContext();
        var subjects = await SyllabusLibrarySeeder.SeedAsync(db);
        var refs = await OfficialSourceSeeder.SeedAsync(db);

        var rrbSpec = ExamCatalogPart2.GetSpecs().First(s => s.Code == "RRB-NTPC-UG");
        var rrbExam = ExamSeedHelper.BuildExam(rrbSpec, refs, subjects);
        db.Exams.Add(rrbExam);

        var userId = Guid.NewGuid();
        db.UserProfiles.Add(new UserProfile
        {
            UserId = userId,
            FullName = "Candidate",
            DateOfBirth = new DateOnly(1999, 6, 15),
            Category = CategoryType.General,
            DomicileState = "Bihar",
            GraduationQualification = "B.Tech",
            EngineeringBranch = "Information Technology",
            Nationality = "Indian"
        });
        db.SavedExams.Add(new SavedExam { UserId = userId, ExamId = rrbExam.Id });
        await db.SaveChangesAsync();

        var dt = new FakeDateTimeProvider { TodayIst = new DateOnly(2026, 11, 11) };
        var engine = new EligibilityEngineService(db, dt);
        var orchestrator = new NotificationOrchestratorService(db, engine, new FakeEmailSender(), dt);

        var firstRunCount = await orchestrator.RunScheduledNotificationChecksAsync();
        var secondRunCount = await orchestrator.RunScheduledNotificationChecksAsync();

        Assert.That(firstRunCount, Is.GreaterThan(0));
        Assert.That(secondRunCount, Is.EqualTo(0));
    }
}
