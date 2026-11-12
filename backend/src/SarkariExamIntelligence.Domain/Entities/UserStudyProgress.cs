using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class UserStudyProgress : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? ExamId { get; set; }
    public Exam? Exam { get; set; }

    public Guid SyllabusTopicId { get; set; }
    public SyllabusTopic SyllabusTopic { get; set; } = null!;

    public StudyProgressStatus Status { get; set; } = StudyProgressStatus.NotStarted;
    public int RevisionCount { get; set; }
    public DateTime? LastRevisedAtUtc { get; set; }
    public string? PreparationNotes { get; set; }
}
