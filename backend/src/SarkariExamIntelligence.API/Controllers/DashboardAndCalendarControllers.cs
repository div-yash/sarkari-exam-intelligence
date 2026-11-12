using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Dashboard;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(IDashboardService dashboardService, ICurrentUserService currentUser)
    {
        _dashboardService = dashboardService;
        _currentUser = currentUser;
    }

    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _dashboardService.GetDashboardSummaryAsync(_currentUser.UserId, cancellationToken);
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(summary));
    }
}

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly ICurrentUserService _currentUser;

    public CalendarController(ICalendarService calendarService, ICurrentUserService currentUser)
    {
        _calendarService = calendarService;
        _currentUser = currentUser;
    }

    [HttpGet("events")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CalendarEventDto>>>> GetEvents([FromQuery] CalendarFilterRequest filter, CancellationToken cancellationToken)
    {
        var events = await _calendarService.GetEventsAsync(filter, _currentUser.UserId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CalendarEventDto>>.Ok(events));
    }

    [HttpGet("export.ics")]
    [AllowAnonymous]
    public async Task<IActionResult> ExportIcs([FromQuery] CalendarFilterRequest filter, CancellationToken cancellationToken)
    {
        var ics = await _calendarService.GenerateIcsExportAsync(filter, _currentUser.UserId, cancellationToken);
        return File(Encoding.UTF8.GetBytes(ics), "text/calendar; charset=utf-8", "sarkari-exam-calendar.ics");
    }
}
