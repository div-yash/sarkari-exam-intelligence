using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class SyllabusSubject : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SubjectGroup SubjectGroup { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<SyllabusTopic> Topics { get; set; } = new List<SyllabusTopic>();
    public ICollection<ExamSyllabus> ExamSyllabi { get; set; } = new List<ExamSyllabus>();
}
