using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Auth;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ICurrentUserService _currentUser;

    public ProfileController(IProfileService profileService, ICurrentUserService currentUser)
    {
        _profileService = profileService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Get(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var profile = await _profileService.GetCurrentProfileAsync(userId, cancellationToken);
        profile.Email = _currentUser.Email ?? string.Empty;
        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Update([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var updated = await _profileService.UpdateCurrentProfileAsync(userId, request, cancellationToken);
        updated.Email = _currentUser.Email ?? string.Empty;
        return Ok(ApiResponse<UserProfileDto>.Ok(updated, "Profile updated successfully."));
    }
}
