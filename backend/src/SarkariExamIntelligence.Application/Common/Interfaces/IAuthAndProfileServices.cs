using SarkariExamIntelligence.Application.DTOs.Auth;

namespace SarkariExamIntelligence.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public interface IProfileService
{
    Task<UserProfileDto> GetCurrentProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto> UpdateCurrentProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
}
