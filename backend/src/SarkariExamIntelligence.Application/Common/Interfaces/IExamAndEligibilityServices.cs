using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Eligibility;
using SarkariExamIntelligence.Application.DTOs.Exams;
using SarkariExamIntelligence.Domain.Entities;

namespace SarkariExamIntelligence.Application.Common.Interfaces;

public interface IExamQueryService
{
    Task<IReadOnlyList<CategorySummaryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<ExamListItemDto>> SearchExamsAsync(ExamFilterRequest filter, Guid? userId, CancellationToken cancellationToken = default);
    Task<ExamDetailDto> GetExamDetailBySlugAsync(string slug, Guid? userId, CancellationToken cancellationToken = default);
    Task<ExamDetailDto> GetExamDetailByIdAsync(Guid examId, Guid? userId, CancellationToken cancellationToken = default);
}

public interface IEligibilityEngine
{
    EligibilityCheckResultDto EvaluateExam(Exam exam, ExamNotification? notification, UserProfile profile, DateOnly referenceToday);
    Task<EligibilitySummaryDto> EvaluateAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<EligibilityCheckResultDto> EvaluateSingleForUserAsync(Guid examId, Guid userId, CancellationToken cancellationToken = default);
    Task<EligibilitySummaryDto> EvaluateAllAdHocAsync(AdHocEligibilityProfileRequest profileRequest, CancellationToken cancellationToken = default);
}
