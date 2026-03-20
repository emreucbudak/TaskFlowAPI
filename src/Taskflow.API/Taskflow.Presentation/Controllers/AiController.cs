using Assistant.Application.Models;
using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskflow.Presentation.Features.CQRS.DailySummary.Queries.Generate;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AiController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetDailySummaryRequest")]
    [HttpPost("daily-summary")]
    public async Task<IActionResult> GenerateDailySummary(CancellationToken ct)
    {
        var result = await mediator.Send(new GenerateDailySummaryQueryRequest(), ct);
        return Ok(result);
    }

    [HttpPost("chatbot")]
    public async Task<IActionResult> AskChatbot(
        [FromBody] AssistantChatRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        return Ok(result);
    }
}
