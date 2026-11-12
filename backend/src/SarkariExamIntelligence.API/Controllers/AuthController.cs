using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Auth;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(
        IAuthService authService,
        IProfileService profileService,
        ICurrentUserService currentUser)
    {
        _authService = authService;
        _profileService = profileService;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Me), ApiResponse<AuthResponse>.Ok(result, "Registration successful."));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful."));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Me(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
        var profile = await _profileService.GetCurrentProfileAsync(userId, cancellationToken);
        profile.Email = _currentUser.Email ?? string.Empty;
        return Ok(ApiResponse<UserProfileDto>.Ok(profile));
    }
}
