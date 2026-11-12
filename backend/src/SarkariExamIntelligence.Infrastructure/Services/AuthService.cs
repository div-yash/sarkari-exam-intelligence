using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Auth;
using SarkariExamIntelligence.Application.Services;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Infrastructure.Identity;

namespace SarkariExamIntelligence.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IApplicationDbContext _db;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IApplicationDbContext db,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            throw new ConflictException($"An account with email '{email}' is already registered.");

        if (!await _roleManager.RoleExistsAsync("Candidate"))
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Candidate"));

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            EmailConfirmed = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToList();
            throw new BadRequestException("Registration failed due to validation rules.", errors);
        }

        await _userManager.AddToRoleAsync(user, "Candidate");

        var profile = new UserProfile
        {
            UserId = user.Id,
            FullName = request.FullName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Category = request.Category,
            IsPwBD = request.IsPwBD,
            IsExServiceman = request.IsExServiceman,
            DomicileState = request.DomicileState.Trim(),
            GraduationQualification = request.GraduationQualification.Trim(),
            EngineeringBranch = string.IsNullOrWhiteSpace(request.EngineeringBranch) ? null : request.EngineeringBranch.Trim(),
            University = string.IsNullOrWhiteSpace(request.University) ? null : request.University.Trim(),
            GraduationPercentage = request.GraduationPercentage,
            GraduationCgpa = request.GraduationCgpa,
            PassingYear = request.PassingYear,
            WorkExperienceMonths = request.WorkExperienceMonths,
            IsWorkingProfessional = request.IsWorkingProfessional,
            CurrentOrganizationType = request.CurrentOrganizationType,
            AdditionalQualifications = request.AdditionalQualifications,
            HasValidGateScore = request.HasValidGateScore,
            GateSubject = request.HasValidGateScore ? request.GateSubject : null,
            GateScore = request.HasValidGateScore ? request.GateScore : null,
            GateYear = request.HasValidGateScore ? request.GateYear : null,
            HasBiharDomicileCertificate = request.HasBiharDomicileCertificate,
            HasValidNclOrEwsCertificate = request.HasValidNclOrEwsCertificate,
            Nationality = string.IsNullOrWhiteSpace(request.Nationality) ? "Indian" : request.Nationality.Trim()
        };

        var pref = new NotificationPreference
        {
            UserId = user.Id,
            EnableInApp = true,
            EnableEmail = true,
            AlertOnNewNotification = true,
            AlertOnApplicationOpen = true,
            Alert30DaysBefore = true,
            Alert7DaysBefore = true,
            Alert3DaysBefore = true,
            Alert1DayBefore = true,
            AlertOnExamDateChange = true,
            AlertOnAdmitCard = true,
            AlertOnResult = true
        };

        _db.UserProfiles.Add(profile);
        _db.NotificationPreferences.Add(pref);
        await _db.SaveChangesAsync(cancellationToken);

        var roles = (await _userManager.GetRolesAsync(user)).ToList();
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, user.FullName, roles);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = _jwtTokenService.GetExpiryUtc(),
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            Profile = ProfileService.MapToDto(profile, user.Email!)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !user.IsActive)
            throw new UnauthorizedException("Invalid email or password.");

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid)
            throw new UnauthorizedException("Invalid email or password.");

        var roles = (await _userManager.GetRolesAsync(user)).ToList();
        var profile = await _db.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, user.FullName, roles);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = _jwtTokenService.GetExpiryUtc(),
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            Profile = profile is null ? null : ProfileService.MapToDto(profile, user.Email!)
        };
    }
}
