using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Interfaces;
using SarkariExamIntelligence.Application.Common.Models;
using SarkariExamIntelligence.Application.DTOs.Exams;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly IExamQueryService _examQuery;
    private readonly ICurrentUserService _currentUser;

    public ExamsController(IExamQueryService examQuery, ICurrentUserService currentUser)
    {
        _examQuery = examQuery;
        _currentUser = currentUser;
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CategorySummaryDto>>>> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _examQuery.GetCategoriesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CategorySummaryDto>>.Ok(result));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<ExamListItemDto>>>> Search([FromQuery] ExamFilterRequest filter, CancellationToken cancellationToken)
    {
        var result = await _examQuery.SearchExamsAsync(filter, _currentUser.UserId, cancellationToken);
        return Ok(ApiResponse<PagedResult<ExamListItemDto>>.Ok(result));
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ExamDetailDto>>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await _examQuery.GetExamDetailBySlugAsync(slug, _currentUser.UserId, cancellationToken);
        return Ok(ApiResponse<ExamDetailDto>.Ok(result));
    }

    [HttpGet("by-id/{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ExamDetailDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _examQuery.GetExamDetailByIdAsync(id, _currentUser.UserId, cancellationToken);
        return Ok(ApiResponse<ExamDetailDto>.Ok(result));
    }
}
