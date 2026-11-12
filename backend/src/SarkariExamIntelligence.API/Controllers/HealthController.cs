using Microsoft.AspNetCore.Mvc;
using SarkariExamIntelligence.Application.Common.Models;

namespace SarkariExamIntelligence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<object>> Get()
    {
        return Ok(ApiResponse<object>.Ok(new
        {
            status = "Healthy",
            service = "Sarkari Exam Intelligence API",
            timestampUtc = DateTime.UtcNow
        }));
    }
}
