using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StatsController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("GetAllWorkersStatsByPeriodQueryRequest")]
    public async Task<IActionResult> GetAllWorkersStatsByPeriodQuery([FromBody] Stats.Application.Features.CQRS.WorkerStats.Queries.GetAll.GetAllWorkersStatsByPeriodQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetWorkerStatsByUserAndPeriodQueryRequest")]
    public async Task<IActionResult> GetWorkerStatsByUserAndPeriodQuery([FromBody] Stats.Application.Features.CQRS.WorkerStats.Queries.GetByUserAndPeriod.GetWorkerStatsByUserAndPeriodQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}

