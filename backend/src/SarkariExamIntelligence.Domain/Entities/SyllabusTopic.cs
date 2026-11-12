using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class SyllabusTopic : BaseEntity
{
    public Guid SyllabusSubjectId { get; set; }
    public SyllabusSubject SyllabusSubject { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = "Moderate"; // Foundation, Moderate, Advanced
    public int EstimatedHours { get; set; } = 4;
    public int DisplayOrder { get; set; }

    public ICollection<UserStudyProgress> UserProgresses { get; set; } = new List<UserStudyProgress>();
}
