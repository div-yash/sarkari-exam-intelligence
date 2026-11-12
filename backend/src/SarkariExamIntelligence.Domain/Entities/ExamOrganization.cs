using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class ExamOrganization : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string OrganizationType { get; set; } = "Central"; // Central, State, Regulatory, PSU, Autonomous
    public string? StateName { get; set; }
    public string OfficialWebsiteUrl { get; set; } = string.Empty;
    public string? RecruitmentPortalUrl { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<OfficialSource> OfficialSources { get; set; } = new List<OfficialSource>();
}
