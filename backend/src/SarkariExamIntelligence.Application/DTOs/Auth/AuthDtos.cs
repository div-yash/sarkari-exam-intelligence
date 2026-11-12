using System.ComponentModel.DataAnnotations;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public CategoryType Category { get; set; } = CategoryType.General;
    public bool IsPwBD { get; set; }
    public bool IsExServiceman { get; set; }

    [Required, MaxLength(100)]
    public string DomicileState { get; set; } = "Bihar";

    [Required, MaxLength(120)]
    public string GraduationQualification { get; set; } = "B.Tech";

    [MaxLength(120)]
    public string? EngineeringBranch { get; set; }

    [MaxLength(200)]
    public string? University { get; set; }

    [Range(0, 100)]
    public decimal? GraduationPercentage { get; set; }

    [Range(0, 10)]
    public decimal? GraduationCgpa { get; set; }

    [Range(1980, 2035)]
    public int? PassingYear { get; set; }

    [Range(0, 600)]
    public int WorkExperienceMonths { get; set; }
    public bool IsWorkingProfessional { get; set; }
    [MaxLength(100)]
    public string? CurrentOrganizationType { get; set; }

    [MaxLength(500)]
    public string? AdditionalQualifications { get; set; }

    public bool HasValidGateScore { get; set; }
    [MaxLength(80)]
    public string? GateSubject { get; set; }
    [Range(0, 1000)]
    public int? GateScore { get; set; }
    [Range(2015, 2030)]
    public int? GateYear { get; set; }

    public bool HasBiharDomicileCertificate { get; set; }
    public bool HasValidNclOrEwsCertificate { get; set; }
    [MaxLength(80)]
    public string Nationality { get; set; } = "Indian";
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public UserProfileDto? Profile { get; set; }
}

public class UserProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public CategoryType Category { get; set; }
    public bool IsPwBD { get; set; }
    public bool IsExServiceman { get; set; }
    public string DomicileState { get; set; } = string.Empty;
    public string GraduationQualification { get; set; } = string.Empty;
    public string? EngineeringBranch { get; set; }
    public string? University { get; set; }
    public decimal? GraduationPercentage { get; set; }
    public decimal? GraduationCgpa { get; set; }
    public int? PassingYear { get; set; }
    public int WorkExperienceMonths { get; set; }
    public bool IsWorkingProfessional { get; set; }
    public string? CurrentOrganizationType { get; set; }
    public string? AdditionalQualifications { get; set; }
    public bool HasValidGateScore { get; set; }
    public string? GateSubject { get; set; }
    public int? GateScore { get; set; }
    public int? GateYear { get; set; }
    public bool HasBiharDomicileCertificate { get; set; }
    public bool HasValidNclOrEwsCertificate { get; set; }
    public string Nationality { get; set; } = "Indian";
    public int ProfileCompletenessPercent { get; set; }
}

public class UpdateProfileRequest
{
    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public GenderType? Gender { get; set; }
    public CategoryType Category { get; set; }
    public bool IsPwBD { get; set; }
    public bool IsExServiceman { get; set; }
    [Required, MaxLength(100)]
    public string DomicileState { get; set; } = string.Empty;
    [Required, MaxLength(120)]
    public string GraduationQualification { get; set; } = string.Empty;
    [MaxLength(120)]
    public string? EngineeringBranch { get; set; }
    [MaxLength(200)]
    public string? University { get; set; }
    [Range(0, 100)]
    public decimal? GraduationPercentage { get; set; }
    [Range(0, 10)]
    public decimal? GraduationCgpa { get; set; }
    [Range(1980, 2035)]
    public int? PassingYear { get; set; }
    [Range(0, 600)]
    public int WorkExperienceMonths { get; set; }
    public bool IsWorkingProfessional { get; set; }
    [MaxLength(100)]
    public string? CurrentOrganizationType { get; set; }
    [MaxLength(500)]
    public string? AdditionalQualifications { get; set; }
    public bool HasValidGateScore { get; set; }
    [MaxLength(80)]
    public string? GateSubject { get; set; }
    [Range(0, 1000)]
    public int? GateScore { get; set; }
    [Range(2015, 2030)]
    public int? GateYear { get; set; }
    public bool HasBiharDomicileCertificate { get; set; }
    public bool HasValidNclOrEwsCertificate { get; set; }
    [MaxLength(80)]
    public string Nationality { get; set; } = "Indian";
}
