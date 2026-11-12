using Microsoft.AspNetCore.Identity;

namespace SarkariExamIntelligence.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
