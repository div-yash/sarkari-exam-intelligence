using SarkariExamIntelligence.Domain.Common;

namespace SarkariExamIntelligence.Domain.Entities;

public class ExamSyllabus : BaseEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public Guid SyllabusSubjectId { get; set; }
    public SyllabusSubject SyllabusSubject { get; set; } = null!;

    public string StageName { get; set; } = "Preliminary & Mains";
    public string? WeightageNotes { get; set; }
    public string? ExamSpecificTopicsNotes { get; set; }
    public string? OfficialSyllabusReferenceUrl { get; set; }
    public int DisplayOrder { get; set; }
}
