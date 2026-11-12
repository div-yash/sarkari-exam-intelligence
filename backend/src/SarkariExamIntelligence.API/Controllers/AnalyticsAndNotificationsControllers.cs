using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Analytics;
using SarkariExamIntelligence.Application.DTOs.Notifications;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    [HttpGet("vacancies")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<VacancyAnalyticsDto>>> GetVacancyAnalytics([FromQuery] string? categorySlug, CancellationToken cancellationToken)
    {
        var data = await _analytics.GetVacancyAnalyticsAsync(categorySlug, cancellationToken);
        return Ok(ApiResponse<VacancyAnalyticsDto>.Ok(data));
    }

    [HttpGet("exams/{examId:guid}/history")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<HistoricalComparisonResponseDto>>> GetHistoricalComparison(Guid examId, CancellationToken cancellationToken)
    {
        var data = await _analytics.GetExamHistoricalComparisonAsync(examId, cancellationToken);
        return Ok(ApiResponse<HistoricalComparisonResponseDto>.Ok(data));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationOrchestratorService _notifications;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(INotificationOrchestratorService notifications, ICurrentUserService currentUser)
    {
        _notifications = notifications;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NotificationDto>>>> GetMyNotifications([FromQuery] bool unreadOnly = false, CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var list = await _notifications.GetUserNotificationsAsync(userId, unreadOnly, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<NotificationDto>>.Ok(list));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var count = await _notifications.GetUnreadCountAsync(userId, cancellationToken);
        return Ok(ApiResponse<int>.Ok(count));
    }

    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _notifications.MarkAsReadAsync(userId, id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Notification marked as read."));
    }

    [HttpPost("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _notifications.MarkAllAsReadAsync(userId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "All notifications marked as read."));
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<ApiResponse<NotificationPreferenceDto>>> GetPreferences(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var prefs = await _notifications.GetPreferencesAsync(userId, cancellationToken);
        return Ok(ApiResponse<NotificationPreferenceDto>.Ok(prefs));
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<ApiResponse<NotificationPreferenceDto>>> UpdatePreferences([FromBody] UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var updated = await _notifications.UpdatePreferencesAsync(userId, request, cancellationToken);
        return Ok(ApiResponse<NotificationPreferenceDto>.Ok(updated, "Notification preferences updated."));
    }

    [HttpPost("run-check")]
    public async Task<ActionResult<ApiResponse<int>>> TriggerMyCheck(CancellationToken cancellationToken)
    {
        var generated = await _notifications.RunScheduledNotificationChecksAsync(cancellationToken);
        return Ok(ApiResponse<int>.Ok(generated, $"Notification check completed ({generated} new alerts)."));
    }
}
