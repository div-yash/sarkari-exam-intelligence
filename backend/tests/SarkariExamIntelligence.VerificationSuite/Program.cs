using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SarkariExamIntelligence.API.Controllers;
using SarkariExamIntelligence.API.Services;
using SarkariExamIntelligence.Application;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Admin;
using SarkariExamIntelligence.Application.DTOs.Auth;
using SarkariExamIntelligence.Application.DTOs.Dashboard;
using SarkariExamIntelligence.Application.DTOs.Exams;
using SarkariExamIntelligence.Application.DTOs.Preparation;
using SarkariExamIntelligence.Application.Services;
using SarkariExamIntelligence.Domain.Enums;
using SarkariExamIntelligence.Infrastructure;
using SarkariExamIntelligence.Infrastructure.Persistence.Seed;

var passed = 0;
var failed = 0;

void AssertTrue(bool condition, string testName, string? detail = null)
{
    if (condition)
    {
        passed++;
        Console.WriteLine($"[PASS] {testName}");
    }
    else
    {
        failed++;
        Console.WriteLine($"[FAIL] {testName}{(detail is null ? "" : $" -> {detail}")}");
    }
}

Console.WriteLine("=== SARKARI EXAM INTELLIGENCE VERIFICATION SUITE ===");

// ---------------------------------------------------------------------------
// PART 1: UNIT TESTS (Age Cutoff Math, Branch Specificity, Bihar Domicile)
// ---------------------------------------------------------------------------
var age30 = EligibilityEngineService.CalculateExactAge(new DateOnly(1996, 8, 1), new DateOnly(2026, 8, 1));
AssertTrue(age30 == (30, 0, 0), "Unit: Exact age on cutoff date (1996-08-01 on 2026-08-01 == 30y 0m 0d)");

var ageOver = EligibilityEngineService.CalculateExactAge(new DateOnly(1996, 7, 31), new DateOnly(2026, 8, 1));
AssertTrue(ageOver == (30, 0, 1), "Unit: Exact age one day over limit (1996-07-31 on 2026-08-01 == 30y 0m 1d)");

var pgKv = SarkariExamIntelligence.Infrastructure.DependencyInjection.NormalizePostgreSqlConnectionString("postgresql://neondb_owner:pass%40123@ep-test.neon.tech/neondb?sslmode=require");
AssertTrue(pgKv.Contains("Host=ep-test.neon.tech;") && pgKv.Contains("Password=pass@123;") && pgKv.Contains("SSL Mode=Require;"),
    "Unit: Neon PostgreSQL URI normalizes cleanly to Npgsql key-value string");
AssertTrue(SarkariExamIntelligence.Infrastructure.DependencyInjection.ResolveProvider(null, "postgresql://u:p@h/db") == "PostgreSQL",
    "Unit: PostgreSQL provider auto-detected from postgres URI");

// ---------------------------------------------------------------------------
// PART 2: IN-PROCESS FULL DI + DATABASE SEEDER + CONTROLLERS + JWT + RBAC
// ---------------------------------------------------------------------------
const string jwtKey = "SarkariExamIntelligence_TestKey_AtLeast32CharactersLong!";
const string jwtIssuer = "SarkariExamIntelligence";
const string jwtAudience = "SarkariExamIntelligenceClient";

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["DatabaseProvider"] = "InMemory",
        ["ConnectionStrings:DefaultConnection"] = $"SeiVerificationDb_{Guid.NewGuid():N}",
        ["DisableBackgroundJobs"] = "true",
        ["Jwt:Key"] = jwtKey,
        ["Jwt:Issuer"] = jwtIssuer,
        ["Jwt:Audience"] = jwtAudience,
        ["Jwt:ExpiryMinutes"] = "120"
    })
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(config);
services.AddLogging();
services.AddDataProtection().UseEphemeralDataProtectionProvider();
services.AddHttpContextAccessor();
services.AddScoped<ICurrentUserService, CurrentUserService>();
services.AddApplication();
services.AddInfrastructure(config);

// Register controllers so DI resolves them cleanly
services.AddTransient<HealthController>();
services.AddTransient<AuthController>();
services.AddTransient<ProfileController>();
services.AddTransient<ExamsController>();
services.AddTransient<EligibilityController>();
services.AddTransient<DashboardController>();
services.AddTransient<CalendarController>();
services.AddTransient<SyllabusController>();
services.AddTransient<PreparationController>();
services.AddTransient<WatchlistController>();
services.AddTransient<AnalyticsController>();
services.AddTransient<NotificationsController>();
services.AddTransient<AdminController>();

using var provider = services.BuildServiceProvider();
await DatabaseSeeder.SeedAsync(provider);

// Helper to switch HTTP context user via real JWT validation
void SetBearerToken(IServiceProvider sp, string? token)
{
    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
    var ctx = new DefaultHttpContext();
    if (!string.IsNullOrEmpty(token))
    {
        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        }, out _);
        ctx.User = principal;
    }
    accessor.HttpContext = ctx;
}

T Unwrap<T>(ActionResult<SarkariExamIntelligence.Application.Common.Models.ApiResponse<T>> actionResult)
{
    var obj = actionResult.Result as ObjectResult;
    var val = obj?.Value as SarkariExamIntelligence.Application.Common.Models.ApiResponse<T>;
    if (val is null || !val.Success) throw new InvalidOperationException($"Unexpected controller response: {val?.Message}");
    return val.Data!;
}

using (var scope = provider.CreateScope())
{
    var sp = scope.ServiceProvider;
    SetBearerToken(sp, null);

    var health = Unwrap(sp.GetRequiredService<HealthController>().Get());
    AssertTrue(health is not null, "Integration: HealthController returns healthy payload");

    var examsCtrl = sp.GetRequiredService<ExamsController>();
    var categories = Unwrap(await examsCtrl.GetCategories(default));
    AssertTrue(categories.Count == 7, $"Integration: 7 Exam Categories seeded (actual={categories.Count})");

    var allExams = Unwrap(await examsCtrl.Search(new ExamFilterRequest { PageSize = 100 }, default));
    AssertTrue(allExams.TotalCount == 65, $"Integration: All 65 Exam Families seeded (actual={allExams.TotalCount})");

    var sylCtrl = sp.GetRequiredService<SyllabusController>();
    var subjects = Unwrap(await sylCtrl.GetLibrary(null, default));
    AssertTrue(subjects.Count == 26, $"Integration: All 26 Syllabus Subjects seeded (actual={subjects.Count})");

    // Candidate Login (B.Tech IT – Bihar Working Professional)
    var authCtrl = sp.GetRequiredService<AuthController>();
    var candLogin = Unwrap(await authCtrl.Login(new LoginRequest { Email = "candidate@sarkariexam.in", Password = "Candidate@12345" }, default));
    AssertTrue(!string.IsNullOrEmpty(candLogin.Token), "Integration: Candidate login issues valid JWT");

    SetBearerToken(sp, candLogin.Token);

    // Dashboard (11 widgets)
    var dashCtrl = sp.GetRequiredService<DashboardController>();
    var dash = Unwrap(await dashCtrl.GetSummary(default));
    AssertTrue(dash.TotalExamsTracked == 65, "Integration: Dashboard TotalExamsTracked == 65");
    AssertTrue(dash.CurrentlyOpenApplications >= 2, $"Integration: Open applications tracked ({dash.CurrentlyOpenApplications})");
    AssertTrue(dash.UrgentDeadlines.Count >= 1, $"Integration: Urgent deadlines active ({dash.UrgentDeadlines.Count})");
    AssertTrue(dash.ExamsMatchingMyQualification > 0, $"Integration: Qualified exams matched ({dash.ExamsMatchingMyQualification})");

    // Eligibility Engine Verification
    var eligCtrl = sp.GetRequiredService<EligibilityController>();
    var eligSummary = Unwrap(await eligCtrl.EvaluateMyProfile(default));
    var results = eligSummary.Results;

    var ibpsSoIt = results.First(r => r.ExamSlug == "ibps-so-it-officer");
    var bpscTech = results.First(r => r.ExamSlug == "bpsc-technical-recruitment");
    var psuGate = results.First(r => r.ExamSlug == "psu-recruitment-through-gate");
    var nda = results.First(r => r.ExamSlug == "upsc-nda");
    var sebiIt = results.First(r => r.ExamSlug == "sebi-grade-a-it");

    AssertTrue(ibpsSoIt.Status == EligibilityResultStatus.Eligible, $"Eligibility: B.Tech IT Eligible for IBPS SO IT Officer (actual={ibpsSoIt.Status})");
    AssertTrue(bpscTech.Status == EligibilityResultStatus.NotEligible, $"Eligibility: B.Tech IT NOT Eligible for Civil/Mech/Elec BPSC Technical AE (actual={bpscTech.Status})");
    AssertTrue(psuGate.Status == EligibilityResultStatus.NotEligible, $"Eligibility: Without GATE score NOT Eligible for PSU via GATE (actual={psuGate.Status})");
    AssertTrue(nda.Status == EligibilityResultStatus.NotEligible, $"Eligibility: 27-yr-old graduate NOT Eligible for NDA (actual={nda.Status})");
    AssertTrue(sebiIt.Status != EligibilityResultStatus.NotEligible, $"Eligibility: B.Tech IT passes SEBI Grade A IT check (actual={sebiIt.Status})");

    // Exam Details (37 fields + 5yr historical)
    var sebiDetail = Unwrap(await examsCtrl.GetBySlug("sebi-grade-a-it", default));
    AssertTrue(sebiDetail.HistoricalRecruitments.Count >= 5, $"Integration: SEBI Grade A IT has 5+ historical rows ({sebiDetail.HistoricalRecruitments.Count})");
    AssertTrue(sebiDetail.HistoricalRecruitments.Any(h => h.CycleYear == 2027 && h.DataStatus == HistoricalDataStatus.AwaitingOfficialNotification),
        "Integration: SEBI Grade A 2027 explicitly marked AwaitingOfficialNotification");

    // Bihar Domicile Reservation Validation (Non-Bihar OBC aged 39 cannot claim Bihar OBC 40yr relaxation)
    var adHocNonBihar = Unwrap(await eligCtrl.EvaluateAdHoc(new SarkariExamIntelligence.Application.DTOs.Eligibility.AdHocEligibilityProfileRequest
    {
        DateOfBirth = new DateOnly(1987, 1, 1),
        Gender = GenderType.Male,
        Category = CategoryType.OBCNCL,
        DomicileState = "Uttar Pradesh",
        GraduationQualification = "B.Tech",
        EngineeringBranch = "Information Technology",
        GraduationPercentage = 72m
    }, default));
    var bpscForUp = adHocNonBihar.Results.First(r => r.ExamSlug == "bpsc-cce");
    AssertTrue(bpscForUp.Status == EligibilityResultStatus.NotEligible && bpscForUp.AgeEvaluation.AppliedRelaxationYears == 0,
        "Eligibility: Non-Bihar OBC candidate cannot claim Bihar state OBC relaxation");

    // Calendar ICS export & Analytics
    var calCtrl = sp.GetRequiredService<CalendarController>();
    var icsFile = await calCtrl.ExportIcs(new CalendarFilterRequest(), default) as FileContentResult;
    var icsText = Encoding.UTF8.GetString(icsFile!.FileContents);
    AssertTrue(icsText.Contains("BEGIN:VCALENDAR") && icsText.Contains("END:VCALENDAR"), "Integration: Calendar .ics export valid");

    var analyticsCtrl = sp.GetRequiredService<AnalyticsController>();
    var analytics = Unwrap(await analyticsCtrl.GetVacancyAnalytics(null, default));
    AssertTrue(analytics.VacanciesByYear.Count >= 4, "Integration: Vacancy analytics aggregated across multi-year cycles");

    // Preparation CRUD (Syllabus Progress + Mock Test)
    var firstTopicId = subjects[0].Topics[0].TopicId;
    var topicProg = Unwrap(await sylCtrl.UpdateTopicProgress(firstTopicId, new UpdateTopicProgressRequest
    {
        Status = StudyProgressStatus.Completed,
        IncrementRevision = true,
        PreparationNotes = "Mastered"
    }, default));
    AssertTrue(topicProg.Status == StudyProgressStatus.Completed && topicProg.RevisionCount >= 1,
        "Integration: Syllabus topic progress & revision tracking updated");

    var prepCtrl = sp.GetRequiredService<PreparationController>();
    var mock = Unwrap(await prepCtrl.AddMockTest(new CreateMockTestRequest
    {
        ExamId = sebiDetail.Id,
        TestTitle = "Verification Mock",
        TestDate = new DateOnly(2026, 11, 11),
        ScoreObtained = 140m,
        MaxScore = 200m,
        AccuracyPercent = 88m,
        TimeTakenMinutes = 90
    }, default));
    AssertTrue(mock.PercentageScore == 70m, "Integration: Mock test percentage calculation accurate");

    // Notifications Deduplication
    var notifCtrl = sp.GetRequiredService<NotificationsController>();
    var dedupCount = Unwrap(await notifCtrl.TriggerMyCheck(default));
    AssertTrue(dedupCount == 0, $"Integration: Re-running notification check creates 0 duplicates (actual={dedupCount})");

    // Verify RBAC Authorization Policy on AdminController
    var authzService = sp.GetRequiredService<IAuthorizationService>();
    var currentPrincipal = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!.User;
    var candAdminAuthz = await authzService.AuthorizeAsync(currentPrincipal, null, "AdminOnly");
    AssertTrue(!candAdminAuthz.Succeeded, "Security: Candidate JWT fails AdminOnly authorization policy");

    // Admin Login + Status Change + Audit Log + Prevent Historical Overwrite
    var adminLogin = Unwrap(await authCtrl.Login(new LoginRequest { Email = "admin@sarkariexam.in", Password = "Admin@12345" }, default));
    SetBearerToken(sp, adminLogin.Token);
    currentPrincipal = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!.User;
    var adminAuthz = await authzService.AuthorizeAsync(currentPrincipal, null, "AdminOnly");
    AssertTrue(adminAuthz.Succeeded, "Security: Admin JWT passes AdminOnly authorization policy");

    var adminCtrl = sp.GetRequiredService<AdminController>();
    var adminOverview = Unwrap(await adminCtrl.GetOverview(default));
    AssertTrue(adminOverview.TotalExams == 65 && adminOverview.Sources.Count == 30,
        "Integration: Admin overview tracks 65 exams & 30 official sources");

    var sscId = allExams.Items.First(e => e.Slug == "ssc-cgl").Id;
    Unwrap(await adminCtrl.ChangeStatus(sscId, new AdminChangeStatusRequest
    {
        Status = ExamStatus.Postponed,
        Reason = "Tier-II rescheduled per SSC notice"
    }, default));

    var auditLogs = Unwrap(await adminCtrl.GetAuditLogs(20, default));
    AssertTrue(auditLogs.Any(l => l.Action == "Postponed"), "Integration: Admin status change persisted to AuditLogs");

    // Rule 10: Never silently overwrite verified historical recruitment data
    var overwriteBlocked = false;
    try
    {
        await adminCtrl.AddHistory(sebiDetail.Id, new AdminAddHistoricalRequest
        {
            CycleYear = 2024,
            DataStatus = HistoricalDataStatus.Verified,
            TotalVacancies = 999
        }, default);
    }
    catch (ConflictException)
    {
        overwriteBlocked = true;
    }
    AssertTrue(overwriteBlocked, "Rule 10: Verified historical recruitment cycle cannot be silently overwritten");

    // Register non-Bihar B.Com candidate (All-India support)
    SetBearerToken(sp, null);
    var createdAction = await authCtrl.Register(new RegisterRequest
    {
        FullName = "Priya Sharma",
        Email = "priya.sharma@example.in",
        Password = "Password@123",
        DateOfBirth = new DateOnly(2000, 3, 10),
        Gender = GenderType.Female,
        Category = CategoryType.General,
        DomicileState = "Maharashtra",
        GraduationQualification = "B.Com",
        GraduationPercentage = 72m,
        Nationality = "Indian"
    }, default);
    AssertTrue(createdAction.Result is CreatedAtActionResult, "Integration: Non-Bihar B.Com candidate registration succeeds");
}

Console.WriteLine($"=== SUMMARY: {passed} Passed, {failed} Failed ===");
return failed == 0 ? 0 : 1;
