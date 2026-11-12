using SarkariExamIntelligence.Application.DTOs.Admin;
using SarkariExamIntelligence.Application.DTOs.Analytics;
using SarkariExamIntelligence.Application.DTOs.Dashboard;
using SarkariExamIntelligence.Application.DTOs.Notifications;
using SarkariExamIntelligence.Application.DTOs.Preparation;

namespace SarkariExamIntelligence.Application.Common.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid? userId, CancellationToken cancellationToken = default);
}

public interface ICalendarService
{
    Task<IReadOnlyList<CalendarEventDto>> GetEventsAsync(CalendarFilterRequest filter, Guid? userId, CancellationToken cancellationToken = default);
    Task<string> GenerateIcsExportAsync(CalendarFilterRequest filter, Guid? userId, CancellationToken cancellationToken = default);
}

public interface IPreparationService
{
    Task<IReadOnlyList<SyllabusSubjectDto>> GetSyllabusLibraryAsync(Guid? userId, Guid? examId, CancellationToken cancellationToken = default);
    Task<SyllabusTopicProgressDto> UpdateTopicProgressAsync(Guid userId, Guid topicId, UpdateTopicProgressRequest request, CancellationToken cancellationToken = default);
    Task<PreparationOverviewDto> GetPreparationOverviewAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<StudyPlanDto> UpsertStudyPlanAsync(Guid userId, UpsertStudyPlanRequest request, CancellationToken cancellationToken = default);
    Task<MockTestResultDto> AddMockTestResultAsync(Guid userId, CreateMockTestRequest request, CancellationToken cancellationToken = default);
    Task DeleteMockTestResultAsync(Guid userId, Guid mockTestId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SavedExamDto>> GetSavedExamsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SavedExamDto> SaveExamAsync(Guid userId, SaveExamRequest request, CancellationToken cancellationToken = default);
    Task RemoveSavedExamAsync(Guid userId, Guid examId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserApplicationDto>> GetUserApplicationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserApplicationDto> UpsertUserApplicationAsync(Guid userId, UpsertUserApplicationRequest request, CancellationToken cancellationToken = default);
}

public interface IAnalyticsService
{
    Task<VacancyAnalyticsDto> GetVacancyAnalyticsAsync(string? categorySlug, CancellationToken cancellationToken = default);
    Task<HistoricalComparisonResponseDto> GetExamHistoricalComparisonAsync(Guid examId, CancellationToken cancellationToken = default);
}

public interface INotificationOrchestratorService
{
    Task<IReadOnlyList<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<NotificationPreferenceDto> GetPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<NotificationPreferenceDto> UpdatePreferencesAsync(Guid userId, UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken = default);
    Task<int> RunScheduledNotificationChecksAsync(CancellationToken cancellationToken = default);
}

public interface ISourceVerificationService
{
    Task<AdminSourceVerificationResultDto> VerifySourceAsync(Guid officialSourceId, string? verifiedByUserId, CancellationToken cancellationToken = default);
}

public interface IAdminService
{
    Task<AdminDashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default);
    Task<Guid> CreateExamAsync(AdminCreateExamRequest request, CancellationToken cancellationToken = default);
    Task UpdateExamAsync(Guid examId, AdminUpdateExamRequest request, CancellationToken cancellationToken = default);
    Task UpdateNotificationDatesAsync(Guid examId, AdminUpdateNotificationDatesRequest request, CancellationToken cancellationToken = default);
    Task UpdateVacanciesAsync(Guid examId, AdminUpdateVacanciesRequest request, CancellationToken cancellationToken = default);
    Task AddHistoricalRecruitmentAsync(Guid examId, AdminAddHistoricalRequest request, CancellationToken cancellationToken = default);
    Task AddSyllabusMappingAsync(Guid examId, AdminAddSyllabusMappingRequest request, CancellationToken cancellationToken = default);
    Task ChangeExamStatusAsync(Guid examId, AdminChangeStatusRequest request, CancellationToken cancellationToken = default);
    Task ApprovePendingRecordAsync(Guid examId, AdminApproveRecordRequest request, CancellationToken cancellationToken = default);
    Task<string> UploadNotificationPdfAsync(Guid examId, string fileName, Stream contentStream, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminOfficialSourceDto>> GetSourcesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminAuditLogDto>> GetAuditLogsAsync(int take = 100, CancellationToken cancellationToken = default);
}

public interface IAdminUserService
{
    Task<IReadOnlyList<AdminUserSummaryDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task ToggleUserActiveAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);
}
