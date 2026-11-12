using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Admin;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IAdminUserService _adminUserService;
    private readonly ISourceVerificationService _sourceVerifier;
    private readonly INotificationOrchestratorService _notificationOrchestrator;
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _db;

    public AdminController(
        IAdminService adminService,
        IAdminUserService adminUserService,
        ISourceVerificationService sourceVerifier,
        INotificationOrchestratorService notificationOrchestrator,
        ICurrentUserService currentUser,
        IApplicationDbContext db)
    {
        _adminService = adminService;
        _adminUserService = adminUserService;
        _sourceVerifier = sourceVerifier;
        _notificationOrchestrator = notificationOrchestrator;
        _currentUser = currentUser;
        _db = db;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<ApiResponse<AdminDashboardOverviewDto>>> GetOverview(CancellationToken cancellationToken)
    {
        var data = await _adminService.GetOverviewAsync(cancellationToken);
        return Ok(ApiResponse<AdminDashboardOverviewDto>.Ok(data));
    }

    [HttpGet("lookups")]
    public async Task<ActionResult<ApiResponse<object>>> GetLookups(CancellationToken cancellationToken)
    {
        var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.DisplayOrder).Select(c => new { c.Id, c.Name, c.Slug }).ToListAsync(cancellationToken);
        var orgs = await _db.ExamOrganizations.AsNoTracking().OrderBy(o => o.ShortCode).Select(o => new { o.Id, o.Name, o.ShortCode }).ToListAsync(cancellationToken);
        var sources = await _db.OfficialSources.AsNoTracking().Include(s => s.Organization).OrderBy(s => s.SourceName).Select(s => new { s.Id, s.SourceName, OrganizationCode = s.Organization.ShortCode }).ToListAsync(cancellationToken);
        var subjects = await _db.SyllabusSubjects.AsNoTracking().OrderBy(s => s.DisplayOrder).Select(s => new { s.Id, s.Name, s.Code, s.SubjectGroup }).ToListAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { categories, orgs, sources, subjects }));
    }

    [HttpPost("exams")]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateExam([FromBody] AdminCreateExamRequest request, CancellationToken cancellationToken)
    {
        var id = await _adminService.CreateExamAsync(request, cancellationToken);
        return Ok(ApiResponse<Guid>.Ok(id, "Examination created."));
    }

    [HttpPut("exams/{examId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateExam(Guid examId, [FromBody] AdminUpdateExamRequest request, CancellationToken cancellationToken)
    {
        await _adminService.UpdateExamAsync(examId, request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId }, "Examination updated."));
    }

    [HttpPut("exams/{examId:guid}/dates")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateDates(Guid examId, [FromBody] AdminUpdateNotificationDatesRequest request, CancellationToken cancellationToken)
    {
        await _adminService.UpdateNotificationDatesAsync(examId, request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId }, "Notification dates updated."));
    }

    [HttpPut("exams/{examId:guid}/vacancies")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateVacancies(Guid examId, [FromBody] AdminUpdateVacanciesRequest request, CancellationToken cancellationToken)
    {
        await _adminService.UpdateVacanciesAsync(examId, request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId }, "Vacancies updated."));
    }

    [HttpPost("exams/{examId:guid}/history")]
    public async Task<ActionResult<ApiResponse<object>>> AddHistory(Guid examId, [FromBody] AdminAddHistoricalRequest request, CancellationToken cancellationToken)
    {
        await _adminService.AddHistoricalRecruitmentAsync(examId, request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId }, "Historical recruitment cycle recorded."));
    }

    [HttpPost("exams/{examId:guid}/syllabus")]
    public async Task<ActionResult<ApiResponse<object>>> AddSyllabusMapping(Guid examId, [FromBody] AdminAddSyllabusMappingRequest request, CancellationToken cancellationToken)
    {
        await _adminService.AddSyllabusMappingAsync(examId, request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId }, "Syllabus mapping added."));
    }

    [HttpPost("exams/{examId:guid}/status")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeStatus(Guid examId, [FromBody] AdminChangeStatusRequest request, CancellationToken cancellationToken)
    {
        await _adminService.ChangeExamStatusAsync(examId, request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId, request.Status }, "Exam status updated."));
    }

    [HttpPost("exams/{examId:guid}/approve")]
    public async Task<ActionResult<ApiResponse<object>>> Approve(Guid examId, [FromBody] AdminApproveRecordRequest request, CancellationToken cancellationToken)
    {
        await _adminService.ApprovePendingRecordAsync(examId, request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId, request.Approve }, request.Approve ? "Record approved & published." : "Record marked pending/rejected."));
    }

    [HttpPost("exams/{examId:guid}/upload-pdf")]
    public async Task<ActionResult<ApiResponse<string>>> UploadPdf(Guid examId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("PDF file is required."));
        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest(ApiResponse<string>.Fail("Only PDF files are allowed."));

        await using var stream = file.OpenReadStream();
        var url = await _adminService.UploadNotificationPdfAsync(examId, file.FileName, stream, cancellationToken);
        return Ok(ApiResponse<string>.Ok(url, "Notification PDF uploaded."));
    }

    [HttpGet("sources")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AdminOfficialSourceDto>>>> GetSources(CancellationToken cancellationToken)
    {
        var list = await _adminService.GetSourcesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AdminOfficialSourceDto>>.Ok(list));
    }

    [HttpPost("sources/{sourceId:guid}/verify")]
    public async Task<ActionResult<ApiResponse<AdminSourceVerificationResultDto>>> VerifySource(Guid sourceId, CancellationToken cancellationToken)
    {
        var res = await _sourceVerifier.VerifySourceAsync(sourceId, _currentUser.UserId?.ToString(), cancellationToken);
        return Ok(ApiResponse<AdminSourceVerificationResultDto>.Ok(res));
    }

    [HttpGet("audit-logs")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AdminAuditLogDto>>>> GetAuditLogs([FromQuery] int take = 100, CancellationToken cancellationToken = default)
    {
        var logs = await _adminService.GetAuditLogsAsync(take, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AdminAuditLogDto>>.Ok(logs));
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AdminUserSummaryDto>>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _adminUserService.GetUsersAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AdminUserSummaryDto>>.Ok(users));
    }

    [HttpPost("users/{userId:guid}/toggle-active")]
    public async Task<ActionResult<ApiResponse<object>>> ToggleUser(Guid userId, [FromQuery] bool isActive, CancellationToken cancellationToken)
    {
        await _adminUserService.ToggleUserActiveAsync(userId, isActive, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { userId, isActive }, "User status updated."));
    }

    [HttpPost("notifications/dispatch-check")]
    public async Task<ActionResult<ApiResponse<int>>> DispatchNotificationJob(CancellationToken cancellationToken)
    {
        var count = await _notificationOrchestrator.RunScheduledNotificationChecksAsync(cancellationToken);
        return Ok(ApiResponse<int>.Ok(count, $"Scheduled notification check generated {count} alerts."));
    }
}
