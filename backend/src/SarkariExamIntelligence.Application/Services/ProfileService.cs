using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Auth;
using SarkariExamIntelligence.Domain.Entities;

namespace SarkariExamIntelligence.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IApplicationDbContext _db;

    public ProfileService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfileDto> GetCurrentProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _db.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Candidate profile not found.");

        return MapToDto(profile, string.Empty);
    }

    public async Task<UserProfileDto> UpdateCurrentProfileAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var profile = await _db.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Candidate profile not found.");

        profile.FullName = request.FullName.Trim();
        profile.DateOfBirth = request.DateOfBirth;
        profile.Gender = request.Gender;
        profile.Category = request.Category;
        profile.IsPwBD = request.IsPwBD;
        profile.IsExServiceman = request.IsExServiceman;
        profile.DomicileState = request.DomicileState.Trim();
        profile.GraduationQualification = request.GraduationQualification.Trim();
        profile.EngineeringBranch = string.IsNullOrWhiteSpace(request.EngineeringBranch) ? null : request.EngineeringBranch.Trim();
        profile.University = string.IsNullOrWhiteSpace(request.University) ? null : request.University.Trim();
        profile.GraduationPercentage = request.GraduationPercentage;
        profile.GraduationCgpa = request.GraduationCgpa;
        profile.PassingYear = request.PassingYear;
        profile.WorkExperienceMonths = request.WorkExperienceMonths;
        profile.IsWorkingProfessional = request.IsWorkingProfessional;
        profile.CurrentOrganizationType = request.CurrentOrganizationType;
        profile.AdditionalQualifications = request.AdditionalQualifications;
        profile.HasValidGateScore = request.HasValidGateScore;
        profile.GateSubject = request.HasValidGateScore ? request.GateSubject : null;
        profile.GateScore = request.HasValidGateScore ? request.GateScore : null;
        profile.GateYear = request.HasValidGateScore ? request.GateYear : null;
        profile.HasBiharDomicileCertificate = request.HasBiharDomicileCertificate;
        profile.HasValidNclOrEwsCertificate = request.HasValidNclOrEwsCertificate;
        profile.Nationality = string.IsNullOrWhiteSpace(request.Nationality) ? "Indian" : request.Nationality.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return MapToDto(profile, string.Empty);
    }

    public static UserProfileDto MapToDto(UserProfile p, string email)
    {
        var filled = 0;
        const int total = 11;
        if (!string.IsNullOrWhiteSpace(p.FullName)) filled++;
        if (p.DateOfBirth.HasValue) filled++;
        if (p.Gender.HasValue) filled++;
        if (!string.IsNullOrWhiteSpace(p.DomicileState)) filled++;
        if (!string.IsNullOrWhiteSpace(p.GraduationQualification)) filled++;
        if (!string.IsNullOrWhiteSpace(p.University)) filled++;
        if (p.GraduationPercentage.HasValue || p.GraduationCgpa.HasValue) filled++;
        if (p.PassingYear.HasValue) filled++;
        if (p.WorkExperienceMonths >= 0) filled++;
        if (!string.IsNullOrWhiteSpace(p.Nationality)) filled++;
        if (!p.GraduationQualification.Contains("Tech", StringComparison.OrdinalIgnoreCase)
            && !p.GraduationQualification.Contains("B.E", StringComparison.OrdinalIgnoreCase)
            || !string.IsNullOrWhiteSpace(p.EngineeringBranch)) filled++;

        return new UserProfileDto
        {
            Id = p.Id,
            UserId = p.UserId,
            Email = email,
            FullName = p.FullName,
            DateOfBirth = p.DateOfBirth,
            Gender = p.Gender,
            Category = p.Category,
            IsPwBD = p.IsPwBD,
            IsExServiceman = p.IsExServiceman,
            DomicileState = p.DomicileState,
            GraduationQualification = p.GraduationQualification,
            EngineeringBranch = p.EngineeringBranch,
            University = p.University,
            GraduationPercentage = p.GraduationPercentage,
            GraduationCgpa = p.GraduationCgpa,
            PassingYear = p.PassingYear,
            WorkExperienceMonths = p.WorkExperienceMonths,
            IsWorkingProfessional = p.IsWorkingProfessional,
            CurrentOrganizationType = p.CurrentOrganizationType,
            AdditionalQualifications = p.AdditionalQualifications,
            HasValidGateScore = p.HasValidGateScore,
            GateSubject = p.GateSubject,
            GateScore = p.GateScore,
            GateYear = p.GateYear,
            HasBiharDomicileCertificate = p.HasBiharDomicileCertificate,
            HasValidNclOrEwsCertificate = p.HasValidNclOrEwsCertificate,
            Nationality = p.Nationality,
            ProfileCompletenessPercent = (int)Math.Round(filled * 100.0 / total)
        };
    }
}
