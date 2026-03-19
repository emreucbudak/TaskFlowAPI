using Assistant.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Taskflow.Presentation.Services;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AiController(
    IDailySummaryService dailySummaryService,
    IAssistantChatService assistantChatService) : ControllerBase
{
    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetDailySummaryRequest")]
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

    [HttpPost("chatbot")]
    public async Task<IActionResult> AskChatbot([FromBody] AssistantChatRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new { Message = "Question alani zorunludur." });
        }

        var response = await assistantChatService.AskAsync(request.Question, cancellationToken);

        return Ok(new AssistantChatResponseDto(
            response.Answer,
            response.Sources
                .Select(source => new AssistantChatSourceDto(
                    source.SourceKey,
                    source.Title,
                    source.ChunkIndex,
                    source.Score))
                .ToArray()));
    }
}

public sealed record AssistantChatRequest(string Question);

public sealed record AssistantChatResponseDto(
    string Answer,
    IReadOnlyCollection<AssistantChatSourceDto> Sources);

public sealed record AssistantChatSourceDto(
    string SourceKey,
    string Title,
    int ChunkIndex,
    double Score);
