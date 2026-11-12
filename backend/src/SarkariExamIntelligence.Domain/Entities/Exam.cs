using SarkariExamIntelligence.Domain.Common;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Domain.Entities;

public class Exam : BaseEntity
{
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid OrganizationId { get; set; }
    public ExamOrganization Organization { get; set; } = null!;

    public Guid OfficialSourceId { get; set; }
    public OfficialSource OfficialSource { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobDesignation { get; set; } = string.Empty;
    public GroupClassification GroupClassification { get; set; } = GroupClassification.NotSpecified;
    public JobNature JobNature { get; set; } = JobNature.Permanent;
    public string OfficialRecruitmentWebsite { get; set; } = string.Empty;

    public bool IsBiharSpecific { get; set; }
    public bool IsItSpecific { get; set; }
    public bool IsGraduateLevel { get; set; } = true;

    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.VerifiedOfficial;
    public DataConfidence DataConfidence { get; set; } = DataConfidence.ConfirmedOfficial;
    public bool IsApprovedForPublish { get; set; } = true;

    public string Summary { get; set; } = string.Empty;
    public string? DefaultSalaryPayLevel { get; set; }
    public string? DefaultSelectionProcedure { get; set; }
    public ExamMode DefaultExamMode { get; set; } = ExamMode.ComputerBasedTest;
    public string? DefaultNegativeMarking { get; set; }

    public ICollection<ExamNotification> Notifications { get; set; } = new List<ExamNotification>();
    public ICollection<EligibilityRule> EligibilityRules { get; set; } = new List<EligibilityRule>();
    public ICollection<EducationalQualification> EducationalQualifications { get; set; } = new List<EducationalQualification>();
    public ICollection<AgeRelaxation> AgeRelaxations { get; set; } = new List<AgeRelaxation>();
    public ICollection<ExamPattern> ExamPatterns { get; set; } = new List<ExamPattern>();
    public ICollection<ExamSyllabus> ExamSyllabi { get; set; } = new List<ExamSyllabus>();
    public ICollection<HistoricalRecruitment> HistoricalRecruitments { get; set; } = new List<HistoricalRecruitment>();
    public ICollection<PreviousYearCutoff> PreviousYearCutoffs { get; set; } = new List<PreviousYearCutoff>();
    public ICollection<PreviousYearPaper> PreviousYearPapers { get; set; } = new List<PreviousYearPaper>();
    public ICollection<SavedExam> SavedByUsers { get; set; } = new List<SavedExam>();
    public ICollection<UserApplication> UserApplications { get; set; } = new List<UserApplication>();
    public ICollection<UserStudyPlan> UserStudyPlans { get; set; } = new List<UserStudyPlan>();
    public ICollection<MockTestResult> MockTestResults { get; set; } = new List<MockTestResult>();
}
