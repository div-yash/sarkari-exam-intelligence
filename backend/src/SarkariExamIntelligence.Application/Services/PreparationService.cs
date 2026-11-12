using Microsoft.EntityFrameworkCore;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.DTOs.Preparation;
using SarkariExamIntelligence.Domain.Entities;
using SarkariExamIntelligence.Domain.Enums;

namespace SarkariExamIntelligence.Application.Services;

public class PreparationService : IPreparationService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public PreparationService(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<IReadOnlyList<SyllabusSubjectDto>> GetSyllabusLibraryAsync(
        Guid? userId,
        Guid? examId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<SyllabusSubject> subjectQuery = _db.SyllabusSubjects
            .AsNoTracking()
            .Include(s => s.Topics);

        if (examId.HasValue)
        {
            var subjectIds = await _db.ExamSyllabi
                .AsNoTracking()
                .Where(es => es.ExamId == examId.Value)
                .Select(es => es.SyllabusSubjectId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (subjectIds.Count > 0)
                subjectQuery = subjectQuery.Where(s => subjectIds.Contains(s.Id));
        }

        var subjects = await subjectQuery
            .OrderBy(s => s.SubjectGroup)
            .ThenBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);

        var progressMap = new Dictionary<Guid, UserStudyProgress>();
        if (userId.HasValue)
        {
            var progresses = await _db.UserStudyProgresses
                .AsNoTracking()
                .Where(p => p.UserId == userId.Value && (examId == null || p.ExamId == examId || p.ExamId == null))
                .ToListAsync(cancellationToken);

            foreach (var p in progresses)
                progressMap[p.SyllabusTopicId] = p;
        }

        return subjects.Select(s =>
        {
            var topics = s.Topics.OrderBy(t => t.DisplayOrder).Select(t =>
            {
                progressMap.TryGetValue(t.Id, out var prog);
                return new SyllabusTopicProgressDto
                {
                    TopicId = t.Id,
                    SubjectId = s.Id,
                    SubjectName = s.Name,
                    Name = t.Name,
                    Description = t.Description,
                    DifficultyLevel = t.DifficultyLevel,
                    EstimatedHours = t.EstimatedHours,
                    Status = prog?.Status ?? StudyProgressStatus.NotStarted,
                    RevisionCount = prog?.RevisionCount ?? 0,
                    LastRevisedAtUtc = prog?.LastRevisedAtUtc,
                    PreparationNotes = prog?.PreparationNotes
                };
            }).ToList();

            return new SyllabusSubjectDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                SubjectGroup = s.SubjectGroup,
                Description = s.Description,
                TotalTopics = topics.Count,
                CompletedTopics = topics.Count(t => t.Status == StudyProgressStatus.Completed),
                InProgressTopics = topics.Count(t => t.Status == StudyProgressStatus.InProgress),
                Topics = topics
            };
        }).ToList();
    }

    public async Task<SyllabusTopicProgressDto> UpdateTopicProgressAsync(
        Guid userId,
        Guid topicId,
        UpdateTopicProgressRequest request,
        CancellationToken cancellationToken = default)
    {
        var topic = await _db.SyllabusTopics
            .Include(t => t.SyllabusSubject)
            .FirstOrDefaultAsync(t => t.Id == topicId, cancellationToken)
            ?? throw new NotFoundException("Syllabus topic not found.");

        var progress = await _db.UserStudyProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.SyllabusTopicId == topicId && p.ExamId == request.ExamId, cancellationToken);

        if (progress is null)
        {
            progress = new UserStudyProgress
            {
                UserId = userId,
                ExamId = request.ExamId,
                SyllabusTopicId = topicId,
                Status = request.Status,
                RevisionCount = request.IncrementRevision ? 1 : 0,
                LastRevisedAtUtc = request.IncrementRevision ? DateTime.UtcNow : null,
                PreparationNotes = request.PreparationNotes
            };
            _db.UserStudyProgresses.Add(progress);
        }
        else
        {
            progress.Status = request.Status;
            if (request.IncrementRevision)
            {
                progress.RevisionCount += 1;
                progress.LastRevisedAtUtc = DateTime.UtcNow;
            }
            if (request.PreparationNotes is not null)
                progress.PreparationNotes = request.PreparationNotes;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new SyllabusTopicProgressDto
        {
            TopicId = topic.Id,
            SubjectId = topic.SyllabusSubjectId,
            SubjectName = topic.SyllabusSubject.Name,
            Name = topic.Name,
            Description = topic.Description,
            DifficultyLevel = topic.DifficultyLevel,
            EstimatedHours = topic.EstimatedHours,
            Status = progress.Status,
            RevisionCount = progress.RevisionCount,
            LastRevisedAtUtc = progress.LastRevisedAtUtc,
            PreparationNotes = progress.PreparationNotes
        };
    }

    public async Task<PreparationOverviewDto> GetPreparationOverviewAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var totalTopics = await _db.SyllabusTopics.CountAsync(cancellationToken);
        var progresses = await _db.UserStudyProgresses.AsNoTracking().Where(p => p.UserId == userId).ToListAsync(cancellationToken);
        var completed = progresses.Count(p => p.Status == StudyProgressStatus.Completed);
        var inProgress = progresses.Count(p => p.Status == StudyProgressStatus.InProgress);
        var revisions = progresses.Sum(p => p.RevisionCount);

        var plans = await _db.UserStudyPlans
            .AsNoTracking()
            .Include(p => p.Exam)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.IsActive)
            .ThenBy(p => p.TargetExamDate)
            .Select(p => new StudyPlanDto
            {
                Id = p.Id,
                ExamId = p.ExamId,
                ExamName = p.Exam.ShortName,
                ExamSlug = p.Exam.Slug,
                PlanTitle = p.PlanTitle,
                DailyTargetMinutes = p.DailyTargetMinutes,
                WeeklyTargetHours = p.WeeklyTargetHours,
                TargetExamDate = p.TargetExamDate,
                TimetableJson = p.TimetableJson,
                IsActive = p.IsActive
            }).ToListAsync(cancellationToken);

        var mocks = await _db.MockTestResults
            .AsNoTracking()
            .Include(m => m.Exam)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.TestDate)
            .Select(m => new MockTestResultDto
            {
                Id = m.Id,
                ExamId = m.ExamId,
                ExamName = m.Exam.ShortName,
                TestTitle = m.TestTitle,
                TestDate = m.TestDate,
                ScoreObtained = m.ScoreObtained,
                MaxScore = m.MaxScore,
                AccuracyPercent = m.AccuracyPercent,
                TimeTakenMinutes = m.TimeTakenMinutes,
                WeakAreasNotes = m.WeakAreasNotes
            }).ToListAsync(cancellationToken);

        var apps = await GetUserApplicationsAsync(userId, cancellationToken);
        var saved = await GetSavedExamsAsync(userId, cancellationToken);

        return new PreparationOverviewDto
        {
            TotalTopicsTracked = totalTopics,
            CompletedTopicsCount = completed,
            InProgressTopicsCount = inProgress,
            OverallCompletionPercent = totalTopics > 0 ? Math.Round(completed * 100m / totalTopics, 1) : 0m,
            TotalRevisions = revisions,
            ActiveStudyPlansCount = plans.Count(p => p.IsActive),
            MockTestsTaken = mocks.Count,
            AverageMockScorePercent = mocks.Count > 0 ? Math.Round(mocks.Average(m => m.PercentageScore), 1) : 0m,
            StudyPlans = plans,
            Applications = apps,
            RecentMockTests = mocks.Take(15).ToList(),
            SavedExams = saved
        };
    }

    public async Task<StudyPlanDto> UpsertStudyPlanAsync(Guid userId, UpsertStudyPlanRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.Id == request.ExamId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        var plan = await _db.UserStudyPlans
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ExamId == request.ExamId, cancellationToken);

        if (plan is null)
        {
            plan = new UserStudyPlan
            {
                UserId = userId,
                ExamId = request.ExamId,
                PlanTitle = request.PlanTitle.Trim(),
                DailyTargetMinutes = request.DailyTargetMinutes,
                WeeklyTargetHours = request.WeeklyTargetHours,
                TargetExamDate = request.TargetExamDate,
                TimetableJson = string.IsNullOrWhiteSpace(request.TimetableJson) ? "[]" : request.TimetableJson,
                IsActive = request.IsActive
            };
            _db.UserStudyPlans.Add(plan);
        }
        else
        {
            plan.PlanTitle = request.PlanTitle.Trim();
            plan.DailyTargetMinutes = request.DailyTargetMinutes;
            plan.WeeklyTargetHours = request.WeeklyTargetHours;
            plan.TargetExamDate = request.TargetExamDate;
            plan.TimetableJson = string.IsNullOrWhiteSpace(request.TimetableJson) ? "[]" : request.TimetableJson;
            plan.IsActive = request.IsActive;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new StudyPlanDto
        {
            Id = plan.Id,
            ExamId = plan.ExamId,
            ExamName = exam.ShortName,
            ExamSlug = exam.Slug,
            PlanTitle = plan.PlanTitle,
            DailyTargetMinutes = plan.DailyTargetMinutes,
            WeeklyTargetHours = plan.WeeklyTargetHours,
            TargetExamDate = plan.TargetExamDate,
            TimetableJson = plan.TimetableJson,
            IsActive = plan.IsActive
        };
    }

    public async Task<MockTestResultDto> AddMockTestResultAsync(Guid userId, CreateMockTestRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.Id == request.ExamId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        if (request.ScoreObtained > request.MaxScore)
            throw new BadRequestException("Score obtained cannot exceed max score.");

        var mock = new MockTestResult
        {
            UserId = userId,
            ExamId = request.ExamId,
            TestTitle = request.TestTitle.Trim(),
            TestDate = request.TestDate,
            ScoreObtained = request.ScoreObtained,
            MaxScore = request.MaxScore,
            AccuracyPercent = request.AccuracyPercent,
            TimeTakenMinutes = request.TimeTakenMinutes,
            WeakAreasNotes = request.WeakAreasNotes
        };

        _db.MockTestResults.Add(mock);
        await _db.SaveChangesAsync(cancellationToken);

        return new MockTestResultDto
        {
            Id = mock.Id,
            ExamId = mock.ExamId,
            ExamName = exam.ShortName,
            TestTitle = mock.TestTitle,
            TestDate = mock.TestDate,
            ScoreObtained = mock.ScoreObtained,
            MaxScore = mock.MaxScore,
            AccuracyPercent = mock.AccuracyPercent,
            TimeTakenMinutes = mock.TimeTakenMinutes,
            WeakAreasNotes = mock.WeakAreasNotes
        };
    }

    public async Task DeleteMockTestResultAsync(Guid userId, Guid mockTestId, CancellationToken cancellationToken = default)
    {
        var mock = await _db.MockTestResults.FirstOrDefaultAsync(m => m.Id == mockTestId && m.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Mock test result not found.");
        _db.MockTestResults.Remove(mock);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SavedExamDto>> GetSavedExamsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var saved = await _db.SavedExams
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Include(s => s.Exam)
                .ThenInclude(e => e.Category)
            .Include(s => s.Exam)
                .ThenInclude(e => e.Organization)
            .Include(s => s.Exam)
                .ThenInclude(e => e.Notifications)
            .OrderByDescending(s => s.SavedAtUtc)
            .ToListAsync(cancellationToken);

        return saved.Select(s =>
        {
            var n = s.Exam.Notifications.FirstOrDefault(x => x.IsCurrentCycle) ?? s.Exam.Notifications.OrderByDescending(x => x.CycleYear).FirstOrDefault();
            return new SavedExamDto
            {
                Id = s.Id,
                ExamId = s.ExamId,
                ExamName = s.Exam.Name,
                ExamSlug = s.Exam.Slug,
                CategoryName = s.Exam.Category.Name,
                OrganizationCode = s.Exam.Organization.ShortCode,
                Status = n?.Status ?? ExamStatus.UpcomingNotification,
                ApplicationEndDate = n?.ApplicationEndDate ?? n?.ExpectedApplicationEndDate,
                ExamDate = n?.ExamDate ?? n?.PrelimExamDate ?? n?.ExpectedExamDate ?? n?.ExpectedPrelimExamDate,
                PriorityLevel = s.PriorityLevel,
                PersonalNotes = s.PersonalNotes,
                SavedAtUtc = s.SavedAtUtc
            };
        }).ToList();
    }

    public async Task<SavedExamDto> SaveExamAsync(Guid userId, SaveExamRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.Organization)
            .Include(e => e.Notifications)
            .FirstOrDefaultAsync(e => e.Id == request.ExamId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        var existing = await _db.SavedExams.FirstOrDefaultAsync(s => s.UserId == userId && s.ExamId == request.ExamId, cancellationToken);
        if (existing is null)
        {
            existing = new SavedExam
            {
                UserId = userId,
                ExamId = request.ExamId,
                PriorityLevel = request.PriorityLevel,
                PersonalNotes = request.PersonalNotes,
                SavedAtUtc = DateTime.UtcNow
            };
            _db.SavedExams.Add(existing);
        }
        else
        {
            existing.PriorityLevel = request.PriorityLevel;
            existing.PersonalNotes = request.PersonalNotes;
        }

        await _db.SaveChangesAsync(cancellationToken);
        var n = exam.Notifications.FirstOrDefault(x => x.IsCurrentCycle) ?? exam.Notifications.FirstOrDefault();

        return new SavedExamDto
        {
            Id = existing.Id,
            ExamId = exam.Id,
            ExamName = exam.Name,
            ExamSlug = exam.Slug,
            CategoryName = exam.Category.Name,
            OrganizationCode = exam.Organization.ShortCode,
            Status = n?.Status ?? ExamStatus.UpcomingNotification,
            ApplicationEndDate = n?.ApplicationEndDate ?? n?.ExpectedApplicationEndDate,
            ExamDate = n?.ExamDate ?? n?.PrelimExamDate,
            PriorityLevel = existing.PriorityLevel,
            PersonalNotes = existing.PersonalNotes,
            SavedAtUtc = existing.SavedAtUtc
        };
    }

    public async Task RemoveSavedExamAsync(Guid userId, Guid examId, CancellationToken cancellationToken = default)
    {
        var existing = await _db.SavedExams.FirstOrDefaultAsync(s => s.UserId == userId && s.ExamId == examId, cancellationToken)
            ?? throw new NotFoundException("Saved exam entry not found.");
        _db.SavedExams.Remove(existing);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserApplicationDto>> GetUserApplicationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = _dateTime.TodayIst;
        var apps = await _db.UserApplications
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Include(a => a.Exam)
                .ThenInclude(e => e.Notifications)
            .OrderByDescending(a => a.UpdatedAtUtc ?? a.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return apps.Select(a =>
        {
            var n = a.Exam.Notifications.FirstOrDefault(x => x.IsCurrentCycle) ?? a.Exam.Notifications.FirstOrDefault();
            var examDate = n?.ExamDate ?? n?.PrelimExamDate ?? n?.ExpectedExamDate ?? n?.ExpectedPrelimExamDate;
            return new UserApplicationDto
            {
                Id = a.Id,
                ExamId = a.ExamId,
                ExamName = a.Exam.Name,
                ExamSlug = a.Exam.Slug,
                TrackingStatus = a.TrackingStatus,
                AppliedDate = a.AppliedDate,
                ApplicationReferenceNumber = a.ApplicationReferenceNumber,
                DocumentsChecklistJson = a.DocumentsChecklistJson,
                Notes = a.Notes,
                ExamDate = examDate,
                DaysToExam = examDate.HasValue ? examDate.Value.DayNumber - today.DayNumber : null
            };
        }).ToList();
    }

    public async Task<UserApplicationDto> UpsertUserApplicationAsync(Guid userId, UpsertUserApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _db.Exams
            .AsNoTracking()
            .Include(e => e.Notifications)
            .FirstOrDefaultAsync(e => e.Id == request.ExamId, cancellationToken)
            ?? throw new NotFoundException("Exam not found.");

        var current = exam.Notifications.FirstOrDefault(x => x.IsCurrentCycle) ?? exam.Notifications.FirstOrDefault();
        var app = await _db.UserApplications.FirstOrDefaultAsync(a => a.UserId == userId && a.ExamId == request.ExamId, cancellationToken);

        if (app is null)
        {
            app = new UserApplication
            {
                UserId = userId,
                ExamId = request.ExamId,
                ExamNotificationId = current?.Id,
                TrackingStatus = request.TrackingStatus,
                AppliedDate = request.AppliedDate,
                ApplicationReferenceNumber = request.ApplicationReferenceNumber,
                DocumentsChecklistJson = string.IsNullOrWhiteSpace(request.DocumentsChecklistJson) ? "[]" : request.DocumentsChecklistJson,
                Notes = request.Notes
            };
            _db.UserApplications.Add(app);
        }
        else
        {
            app.TrackingStatus = request.TrackingStatus;
            app.AppliedDate = request.AppliedDate;
            app.ApplicationReferenceNumber = request.ApplicationReferenceNumber;
            app.DocumentsChecklistJson = string.IsNullOrWhiteSpace(request.DocumentsChecklistJson) ? "[]" : request.DocumentsChecklistJson;
            app.Notes = request.Notes;
        }

        await _db.SaveChangesAsync(cancellationToken);
        var today = _dateTime.TodayIst;
        var examDate = current?.ExamDate ?? current?.PrelimExamDate ?? current?.ExpectedExamDate;

        return new UserApplicationDto
        {
            Id = app.Id,
            ExamId = app.ExamId,
            ExamName = exam.Name,
            ExamSlug = exam.Slug,
            TrackingStatus = app.TrackingStatus,
            AppliedDate = app.AppliedDate,
            ApplicationReferenceNumber = app.ApplicationReferenceNumber,
            DocumentsChecklistJson = app.DocumentsChecklistJson,
            Notes = app.Notes,
            ExamDate = examDate,
            DaysToExam = examDate.HasValue ? examDate.Value.DayNumber - today.DayNumber : null
        };
    }
}
