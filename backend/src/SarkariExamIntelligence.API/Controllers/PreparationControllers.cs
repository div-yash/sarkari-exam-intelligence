using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Preparation;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyllabusController : ControllerBase
{
    private readonly IPreparationService _prep;
    private readonly ICurrentUserService _currentUser;

    public SyllabusController(IPreparationService prep, ICurrentUserService currentUser)
    {
        _prep = prep;
        _currentUser = currentUser;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SyllabusSubjectDto>>>> GetLibrary([FromQuery] Guid? examId, CancellationToken cancellationToken)
    {
        var data = await _prep.GetSyllabusLibraryAsync(_currentUser.UserId, examId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SyllabusSubjectDto>>.Ok(data));
    }

    [HttpPut("topics/{topicId:guid}/progress")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SyllabusTopicProgressDto>>> UpdateTopicProgress(Guid topicId, [FromBody] UpdateTopicProgressRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var updated = await _prep.UpdateTopicProgressAsync(userId, topicId, request, cancellationToken);
        return Ok(ApiResponse<SyllabusTopicProgressDto>.Ok(updated, "Topic progress saved."));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PreparationController : ControllerBase
{
    private readonly IPreparationService _prep;
    private readonly ICurrentUserService _currentUser;

    public PreparationController(IPreparationService prep, ICurrentUserService currentUser)
    {
        _prep = prep;
        _currentUser = currentUser;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<ApiResponse<PreparationOverviewDto>>> GetOverview(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var overview = await _prep.GetPreparationOverviewAsync(userId, cancellationToken);
        return Ok(ApiResponse<PreparationOverviewDto>.Ok(overview));
    }

    [HttpPut("study-plan")]
    public async Task<ActionResult<ApiResponse<StudyPlanDto>>> UpsertStudyPlan([FromBody] UpsertStudyPlanRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var plan = await _prep.UpsertStudyPlanAsync(userId, request, cancellationToken);
        return Ok(ApiResponse<StudyPlanDto>.Ok(plan, "Study plan saved."));
    }

    [HttpPost("mock-tests")]
    public async Task<ActionResult<ApiResponse<MockTestResultDto>>> AddMockTest([FromBody] CreateMockTestRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var result = await _prep.AddMockTestResultAsync(userId, request, cancellationToken);
        return Ok(ApiResponse<MockTestResultDto>.Ok(result, "Mock test score recorded."));
    }

    [HttpDelete("mock-tests/{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMockTest(Guid id, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _prep.DeleteMockTestResultAsync(userId, id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Mock test deleted."));
    }

    [HttpGet("applications")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserApplicationDto>>>> GetApplications(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var apps = await _prep.GetUserApplicationsAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UserApplicationDto>>.Ok(apps));
    }

    [HttpPut("applications")]
    public async Task<ActionResult<ApiResponse<UserApplicationDto>>> UpsertApplication([FromBody] UpsertUserApplicationRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var app = await _prep.UpsertUserApplicationAsync(userId, request, cancellationToken);
        return Ok(ApiResponse<UserApplicationDto>.Ok(app, "Application tracking status updated."));
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WatchlistController : ControllerBase
{
    private readonly IPreparationService _prep;
    private readonly ICurrentUserService _currentUser;

    public WatchlistController(IPreparationService prep, ICurrentUserService currentUser)
    {
        _prep = prep;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SavedExamDto>>>> GetSaved(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var list = await _prep.GetSavedExamsAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SavedExamDto>>.Ok(list));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SavedExamDto>>> Save([FromBody] SaveExamRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var saved = await _prep.SaveExamAsync(userId, request, cancellationToken);
        return Ok(ApiResponse<SavedExamDto>.Ok(saved, "Exam added to watchlist."));
    }

    [HttpDelete("{examId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Remove(Guid examId, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        await _prep.RemoveSavedExamAsync(userId, examId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { examId }, "Exam removed from watchlist."));
    }
}
