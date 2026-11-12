using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Eligibility;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EligibilityController : ControllerBase
{
    private readonly IEligibilityEngine _eligibility;
    private readonly ICurrentUserService _currentUser;

    public EligibilityController(IEligibilityEngine eligibility, ICurrentUserService currentUser)
    {
        _eligibility = eligibility;
        _currentUser = currentUser;
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<EligibilitySummaryDto>>> EvaluateMyProfile(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var result = await _eligibility.EvaluateAllForUserAsync(userId, cancellationToken);
        return Ok(ApiResponse<EligibilitySummaryDto>.Ok(result));
    }

    [HttpGet("my/{examId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<EligibilityCheckResultDto>>> EvaluateMyExam(Guid examId, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var result = await _eligibility.EvaluateSingleForUserAsync(examId, userId, cancellationToken);
        return Ok(ApiResponse<EligibilityCheckResultDto>.Ok(result));
    }

    [HttpPost("evaluate")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<EligibilitySummaryDto>>> EvaluateAdHoc([FromBody] AdHocEligibilityProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _eligibility.EvaluateAllAdHocAsync(request, cancellationToken);
        return Ok(ApiResponse<EligibilitySummaryDto>.Ok(result));
    }
}
