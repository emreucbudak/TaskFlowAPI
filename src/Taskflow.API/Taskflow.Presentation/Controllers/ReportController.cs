using FlashMediator;
using Microsoft.AspNetCore.Mvc;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportController(IMediator mediator) : ControllerBase
{
    [HttpPost("CreateReportCommandRequest")]
    public async Task<IActionResult> CreateReportCommand([FromBody] Report.Application.Features.CQRS.Reports.Command.Create.CreateReportCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteReportCommandRequest")]
    public async Task<IActionResult> DeleteReportCommand([FromBody] Report.Application.Features.CQRS.Reports.Command.Delete.DeleteReportCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("GetAllReportsQueryRequest")]
    public async Task<IActionResult> GetAllReportsQuery([FromBody] Report.Application.Features.CQRS.Reports.Query.GetAll.GetAllReportsQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("GetReportByIdQueryRequest")]
    public async Task<IActionResult> GetReportByIdQuery([FromBody] Report.Application.Features.CQRS.Reports.Query.GetById.GetReportByIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("UpdateReportCommandRequest")]
    public async Task<IActionResult> UpdateReportCommand([FromBody] Report.Application.Features.CQRS.Reports.Command.Update.UpdateReportCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}

