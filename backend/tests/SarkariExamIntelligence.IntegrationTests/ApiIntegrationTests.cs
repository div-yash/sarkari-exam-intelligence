using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SarkariExamIntelligence.Application;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Auth;
using SarkariExamIntelligence.Application.DTOs.Exams;
using SarkariExamIntelligence.Domain.Enums;
using SarkariExamIntelligence.Infrastructure;
using SarkariExamIntelligence.Infrastructure.Persistence.Seed;

namespace SarkariExamIntelligence.IntegrationTests;

[TestFixture]
public class ApiIntegrationTests
{
    private ServiceProvider _provider = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DatabaseProvider"] = "InMemory",
                ["ConnectionStrings:DefaultConnection"] = $"IntegrationDb_{Guid.NewGuid():N}",
                ["DisableBackgroundJobs"] = "true",
                ["Jwt:Key"] = "SarkariExamIntelligence_IntegrationKey_AtLeast32Chars!",
                ["Jwt:Issuer"] = "SarkariExamIntelligence",
                ["Jwt:Audience"] = "SarkariExamIntelligenceClient"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(config);

        _provider = services.BuildServiceProvider();
        await DatabaseSeeder.SeedAsync(_provider);
    }

    [OneTimeTearDown]
    public void Teardown() => _provider.Dispose();

    [Test]
    public async Task SeededCatalog_ContainsAll65Exams_And_26SyllabusSubjects()
    {
        using var scope = _provider.CreateScope();
        var exams = scope.ServiceProvider.GetRequiredService<IExamQueryService>();
        var prep = scope.ServiceProvider.GetRequiredService<IPreparationService>();

        var categories = await exams.GetCategoriesAsync();
        var paged = await exams.SearchExamsAsync(new ExamFilterRequest { PageSize = 100 }, null);
        var subjects = await prep.GetSyllabusLibraryAsync(null, null);

        Assert.That(categories.Count, Is.EqualTo(7));
        Assert.That(paged.TotalCount, Is.EqualTo(65));
        Assert.That(subjects.Count, Is.EqualTo(26));
    }

    [Test]
    public async Task RegisterAndEvaluate_WorksForNonBiharCandidate()
    {
        using var scope = _provider.CreateScope();
        var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var eligibility = scope.ServiceProvider.GetRequiredService<IEligibilityEngine>();

        var reg = await auth.RegisterAsync(new RegisterRequest
        {
            FullName = "Ananya Verma",
            Email = $"ananya.{Guid.NewGuid():N}@example.in",
            Password = "Password@123",
            DateOfBirth = new DateOnly(2000, 4, 20),
            Gender = GenderType.Female,
            Category = CategoryType.General,
            DomicileState = "Uttar Pradesh",
            GraduationQualification = "B.Sc",
            GraduationPercentage = 74m,
            Nationality = "Indian"
        });

        var summary = await eligibility.EvaluateAllForUserAsync(reg.UserId);
        var ibpsIt = summary.Results.First(r => r.ExamSlug == "ibps-so-it-officer");
        Assert.That(ibpsIt.Status, Is.EqualTo(EligibilityResultStatus.NotEligible));
    }
}
