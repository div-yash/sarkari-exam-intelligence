using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;
using SarkariExamIntelligence.Infrastructure.Identity;

namespace SarkariExamIntelligence.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        if (db.Database.IsSqlServer())
        {
            await db.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        // 1. Roles & Sample Accounts
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        foreach (var role in new[] { "Admin", "Candidate" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var adminEmail = "admin@sarkariexam.in";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Sarkari Exam Admin",
                EmailConfirmed = true,
                IsActive = true
            };
            await userManager.CreateAsync(adminUser, "Admin@12345");
            await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Candidate" });
            db.UserProfiles.Add(new UserProfile
            {
                UserId = adminUser.Id,
                FullName = adminUser.FullName,
                DateOfBirth = new DateOnly(1995, 5, 10),
                Gender = GenderType.Male,
                Category = CategoryType.General,
                DomicileState = "Bihar",
                GraduationQualification = "B.Tech",
                EngineeringBranch = "Information Technology",
                University = "Patna University / NIT Patna",
                GraduationPercentage = 81.5m,
                PassingYear = 2017,
                WorkExperienceMonths = 60,
                IsWorkingProfessional = true,
                HasBiharDomicileCertificate = true,
                Nationality = "Indian"
            });
            await db.SaveChangesAsync(cancellationToken);
        }

        var candidateEmail = "candidate@sarkariexam.in";
        ApplicationUser? primaryCandidate = await userManager.FindByEmailAsync(candidateEmail);
        if (primaryCandidate is null)
        {
            primaryCandidate = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = candidateEmail,
                Email = candidateEmail,
                FullName = "Aarav Kumar (B.Tech IT – Bihar)",
                EmailConfirmed = true,
                IsActive = true
            };
            await userManager.CreateAsync(primaryCandidate, "Candidate@12345");
            await userManager.AddToRoleAsync(primaryCandidate, "Candidate");
            db.UserProfiles.Add(new UserProfile
            {
                UserId = primaryCandidate.Id,
                FullName = primaryCandidate.FullName,
                DateOfBirth = new DateOnly(1999, 6, 15),
                Gender = GenderType.Male,
                Category = CategoryType.OBCNCL,
                DomicileState = "Bihar",
                GraduationQualification = "B.Tech",
                EngineeringBranch = "Information Technology",
                University = "Bihar Engineering University / Aryabhatta Knowledge University",
                GraduationPercentage = 78.4m,
                GraduationCgpa = 8.25m,
                PassingYear = 2022,
                WorkExperienceMonths = 36,
                IsWorkingProfessional = true,
                CurrentOrganizationType = "Private IT Services",
                AdditionalQualifications = "AWS Certified Solutions Architect, Gate Aspirant",
                HasValidGateScore = false,
                HasBiharDomicileCertificate = true,
                HasValidNclOrEwsCertificate = true,
                Nationality = "Indian"
            });
            db.NotificationPreferences.Add(new NotificationPreference
            {
                UserId = primaryCandidate.Id,
                EnableInApp = true,
                EnableEmail = true,
                AlertOnNewNotification = true,
                AlertOnApplicationOpen = true,
                Alert30DaysBefore = true,
                Alert7DaysBefore = true,
                Alert3DaysBefore = true,
                Alert1DayBefore = true,
                AlertOnExamDateChange = true,
                AlertOnAdmitCard = true,
                AlertOnResult = true
            });
            await db.SaveChangesAsync(cancellationToken);
        }

        // 2. Syllabus Library (26 subjects) & Official Sources (30 orgs)
        var subjects = await SyllabusLibrarySeeder.SeedAsync(db, cancellationToken);
        var orgRefs = await OfficialSourceSeeder.SeedAsync(db, cancellationToken);

        // 3. Seed All 65 Exam Families
        var existingCodes = (await db.Exams.Select(e => e.Code).ToListAsync(cancellationToken)).ToHashSet();
        var allSpecs = ExamCatalogPart1.GetSpecs().Concat(ExamCatalogPart2.GetSpecs()).ToList();

        var addedExams = 0;
        foreach (var spec in allSpecs)
        {
            if (existingCodes.Contains(spec.Code)) continue;
            var exam = ExamSeedHelper.BuildExam(spec, orgRefs, subjects);
            db.Exams.Add(exam);
            addedExams++;
        }

        if (addedExams > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} official government examination families.", addedExams);
        }

        // 4. Sample Watchlist, Preparation Plan & Notifications for primary candidate
        if (!await db.SavedExams.AnyAsync(s => s.UserId == primaryCandidate.Id, cancellationToken))
        {
            var featuredSlugs = new List<string> { "upsc-civil-services", "ssc-cgl", "ibps-so-it-officer", "sebi-grade-a-it", "bpsc-cce", "gate-cs", "rrb-ntpc-undergraduate", "nic-scientist-b" };
            var featuredExams = await db.Exams.Where(e => featuredSlugs.Contains(e.Slug)).ToListAsync(cancellationToken);

            foreach (var fe in featuredExams)
            {
                db.SavedExams.Add(new SavedExam
                {
                    UserId = primaryCandidate.Id,
                    ExamId = fe.Id,
                    PriorityLevel = fe.IsItSpecific || fe.IsBiharSpecific ? "High" : "Medium",
                    PersonalNotes = fe.IsItSpecific ? "Core IT stream target for working professional schedule." : "Graduate/Bihar state priority exam."
                });
            }

            var sebi = featuredExams.FirstOrDefault(e => e.Slug == "sebi-grade-a-it");
            if (sebi is not null)
            {
                db.UserStudyPlans.Add(new UserStudyPlan
                {
                    UserId = primaryCandidate.Id,
                    ExamId = sebi.Id,
                    PlanTitle = "Working Professional Evening + Weekend Plan (SEBI IT & NIC)",
                    DailyTargetMinutes = 150,
                    WeeklyTargetHours = 22,
                    TargetExamDate = new DateOnly(2027, 2, 15),
                    TimetableJson = "[{\"day\":\"Mon-Fri\",\"slot\":\"06:30–08:00 & 21:00–22:00\",\"focus\":\"DSA, DBMS, OS & Mock Drills\"},{\"day\":\"Sat-Sun\",\"slot\":\"08:00–13:00\",\"focus\":\"Full Length Mocks + Financial Awareness & Bihar GK\"}]",
                    IsActive = true
                });

                db.MockTestResults.AddRange(
                    new MockTestResult
                    {
                        UserId = primaryCandidate.Id,
                        ExamId = sebi.Id,
                        TestTitle = "SEBI IT Phase-I Full Mock #1",
                        TestDate = new DateOnly(2026, 10, 28),
                        ScoreObtained = 128.5m,
                        MaxScore = 200m,
                        AccuracyPercent = 82.5m,
                        TimeTakenMinutes = 95,
                        WeakAreasNotes = "Review Financial Awareness & NP-Completeness."
                    },
                    new MockTestResult
                    {
                        UserId = primaryCandidate.Id,
                        ExamId = sebi.Id,
                        TestTitle = "SEBI IT Phase-I Full Mock #2",
                        TestDate = new DateOnly(2026, 11, 6),
                        ScoreObtained = 141.0m,
                        MaxScore = 200m,
                        AccuracyPercent = 86.0m,
                        TimeTakenMinutes = 92,
                        WeakAreasNotes = "Strong DBMS/SQL; polish SEBI regulatory updates."
                    });
            }

            var rrbUg = featuredExams.FirstOrDefault(e => e.Slug == "rrb-ntpc-undergraduate");
            if (rrbUg is not null)
            {
                db.UserApplications.Add(new UserApplication
                {
                    UserId = primaryCandidate.Id,
                    ExamId = rrbUg.Id,
                    TrackingStatus = ApplicationTrackingStatus.Preparing,
                    DocumentsChecklistJson = "[{\"doc\":\"10th/12th & B.Tech Degree Marksheets\",\"done\":true},{\"doc\":\"Bihar Domicile Certificate\",\"done\":true},{\"doc\":\"OBC-NCL Central Format Certificate\",\"done\":true},{\"doc\":\"Passport Photo & Signature Scan\",\"done\":false}]",
                    Notes = "Verify fee refund bank account details before final submit."
                });
            }

            // Mark a few initial syllabus topics as Completed / InProgress so progress charts are informative immediately
            var sampleTopics = await db.SyllabusTopics.OrderBy(t => t.DisplayOrder).Take(12).ToListAsync(cancellationToken);
            for (var i = 0; i < sampleTopics.Count; i++)
            {
                db.UserStudyProgresses.Add(new UserStudyProgress
                {
                    UserId = primaryCandidate.Id,
                    SyllabusTopicId = sampleTopics[i].Id,
                    Status = i < 7 ? StudyProgressStatus.Completed : StudyProgressStatus.InProgress,
                    RevisionCount = i < 7 ? 2 : 1,
                    LastRevisedAtUtc = DateTime.UtcNow.AddDays(-i),
                    PreparationNotes = "Initial baseline tracked."
                });
            }

            await db.SaveChangesAsync(cancellationToken);

            // Generate initial deduplicated alerts via NotificationOrchestratorService
            var notifService = scope.ServiceProvider.GetRequiredService<INotificationOrchestratorService>();
            await notifService.RunScheduledNotificationChecksAsync(cancellationToken);
        }
    }
}
