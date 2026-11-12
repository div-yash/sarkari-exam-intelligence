using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class EducationalQualification : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public string DegreeLevel { get; set; } = string.Empty; // Undergraduate, Engineering, PostGraduate, Diploma, 12th, 10th
    public string DegreeName { get; set; } = string.Empty;
    public string? AcceptedBranchesCsv { get; set; }
    public decimal? MinimumPercentage { get; set; }
    public bool IsMandatory { get; set; } = true;
    public string? Notes { get; set; }
}
