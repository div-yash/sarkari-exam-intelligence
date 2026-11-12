using System.ComponentModel.DataAnnotations;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.DTOs.Preparation;

public class SyllabusSubjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SubjectGroup SubjectGroup { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TotalTopics { get; set; }
    public int CompletedTopics { get; set; }
    public int InProgressTopics { get; set; }
    public IReadOnlyList<SyllabusTopicProgressDto> Topics { get; set; } = Array.Empty<SyllabusTopicProgressDto>();
}

public class SyllabusTopicProgressDto
{
    public Guid TopicId { get; set; }
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public int EstimatedHours { get; set; }
    public StudyProgressStatus Status { get; set; } = StudyProgressStatus.NotStarted;
    public int RevisionCount { get; set; }
    public DateTime? LastRevisedAtUtc { get; set; }
    public string? PreparationNotes { get; set; }
}

public class UpdateTopicProgressRequest
{
    public Guid? ExamId { get; set; }
    public StudyProgressStatus Status { get; set; }
    public bool IncrementRevision { get; set; }
    [MaxLength(2000)]
    public string? PreparationNotes { get; set; }
}

public class StudyPlanDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamSlug { get; set; } = string.Empty;
    public string PlanTitle { get; set; } = string.Empty;
    public int DailyTargetMinutes { get; set; }
    public int WeeklyTargetHours { get; set; }
    public DateOnly? TargetExamDate { get; set; }
    public string TimetableJson { get; set; } = "[]";
    public bool IsActive { get; set; }
}

public class UpsertStudyPlanRequest
{
    [Required]
    public Guid ExamId { get; set; }
    [Required, MaxLength(200)]
    public string PlanTitle { get; set; } = string.Empty;
    [Range(15, 960)]
    public int DailyTargetMinutes { get; set; } = 120;
    [Range(1, 100)]
    public int WeeklyTargetHours { get; set; } = 15;
    public DateOnly? TargetExamDate { get; set; }
    public string TimetableJson { get; set; } = "[]";
    public bool IsActive { get; set; } = true;
}

public class MockTestResultDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string TestTitle { get; set; } = string.Empty;
    public DateOnly TestDate { get; set; }
    public decimal ScoreObtained { get; set; }
    public decimal MaxScore { get; set; }
    public decimal PercentageScore => MaxScore > 0 ? Math.Round(ScoreObtained * 100m / MaxScore, 2) : 0;
    public decimal AccuracyPercent { get; set; }
    public int TimeTakenMinutes { get; set; }
    public string? WeakAreasNotes { get; set; }
}

public class CreateMockTestRequest
{
    [Required]
    public Guid ExamId { get; set; }
    [Required, MaxLength(200)]
    public string TestTitle { get; set; } = string.Empty;
    [Required]
    public DateOnly TestDate { get; set; }
    [Range(0, 2000)]
    public decimal ScoreObtained { get; set; }
    [Range(1, 2000)]
    public decimal MaxScore { get; set; }
    [Range(0, 100)]
    public decimal AccuracyPercent { get; set; }
    [Range(1, 600)]
    public int TimeTakenMinutes { get; set; }
    [MaxLength(1000)]
    public string? WeakAreasNotes { get; set; }
}

public class SavedExamDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamSlug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string OrganizationCode { get; set; } = string.Empty;
    public ExamStatus Status { get; set; }
    public DateOnly? ApplicationEndDate { get; set; }
    public DateOnly? ExamDate { get; set; }
    public string PriorityLevel { get; set; } = "High";
    public string? PersonalNotes { get; set; }
    public DateTime SavedAtUtc { get; set; }
}

public class SaveExamRequest
{
    [Required]
    public Guid ExamId { get; set; }
    public string PriorityLevel { get; set; } = "High";
    [MaxLength(1000)]
    public string? PersonalNotes { get; set; }
}

public class UserApplicationDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string ExamSlug { get; set; } = string.Empty;
    public ApplicationTrackingStatus TrackingStatus { get; set; }
    public DateOnly? AppliedDate { get; set; }
    public string? ApplicationReferenceNumber { get; set; }
    public string DocumentsChecklistJson { get; set; } = "[]";
    public string? Notes { get; set; }
    public DateOnly? ExamDate { get; set; }
    public int? DaysToExam { get; set; }
}

public class UpsertUserApplicationRequest
{
    [Required]
    public Guid ExamId { get; set; }
    public ApplicationTrackingStatus TrackingStatus { get; set; }
    public DateOnly? AppliedDate { get; set; }
    [MaxLength(120)]
    public string? ApplicationReferenceNumber { get; set; }
    public string DocumentsChecklistJson { get; set; } = "[]";
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class PreparationOverviewDto
{
    public int TotalTopicsTracked { get; set; }
    public int CompletedTopicsCount { get; set; }
    public int InProgressTopicsCount { get; set; }
    public decimal OverallCompletionPercent { get; set; }
    public int TotalRevisions { get; set; }
    public int ActiveStudyPlansCount { get; set; }
    public int MockTestsTaken { get; set; }
    public decimal AverageMockScorePercent { get; set; }

    public IReadOnlyList<StudyPlanDto> StudyPlans { get; set; } = Array.Empty<StudyPlanDto>();
    public IReadOnlyList<UserApplicationDto> Applications { get; set; } = Array.Empty<UserApplicationDto>();
    public IReadOnlyList<MockTestResultDto> RecentMockTests { get; set; } = Array.Empty<MockTestResultDto>();
    public IReadOnlyList<SavedExamDto> SavedExams { get; set; } = Array.Empty<SavedExamDto>();
}
