using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taskflow.Presentation.Services;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AiController(IDailySummaryService dailySummaryService) : ControllerBase
{
    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("daily-summary")]
    public async Task<IActionResult> GenerateDailySummary(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var companyIdClaim = User.FindFirstValue("companyId");

        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(companyIdClaim))
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var companyId = Guid.Parse(companyIdClaim);
        var isDepartmentLeader = User.FindFirstValue("isDepartmentLeader") == "true";

        Guid? departmentId = null;
        var departmentIdClaim = User.FindFirstValue("departmentId");
        if (!string.IsNullOrEmpty(departmentIdClaim))
            departmentId = Guid.Parse(departmentIdClaim);

        var summary = await dailySummaryService.GenerateDailySummaryAsync(
            userId,
            companyId,
            isDepartmentLeader,
            departmentId,
            cancellationToken);

        return Ok(new { Summary = summary });
    }
}
