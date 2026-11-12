using Microsoft.Extensions.DependencyInjection;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Services;

namespace SarkariExamIntelligence.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IEligibilityEngine, EligibilityEngineService>();
        services.AddScoped<IExamQueryService, ExamQueryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IPreparationService, PreparationService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<INotificationOrchestratorService, NotificationOrchestratorService>();
        services.AddScoped<IAdminService, AdminService>();
        return services;
    }
}
