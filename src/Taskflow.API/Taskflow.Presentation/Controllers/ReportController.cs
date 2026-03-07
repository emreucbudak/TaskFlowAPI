using FlashMediator;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportController(IMediator mediator, UserManager<User> userManager) : ControllerBase
{
    [Authorize(Policy = "SubscribedWorkerPolicy")]
    [HttpPost("CreateReportCommandRequest")]
    public async Task<IActionResult> CreateReportCommand([FromBody] Report.Application.Features.CQRS.Reports.Command.Create.CreateReportCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteReportCommandRequest")]
    public async Task<IActionResult> DeleteReportCommand([FromBody] Report.Application.Features.CQRS.Reports.Command.Delete.DeleteReportCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetAllReportsQueryRequest")]
    public async Task<IActionResult> GetAllReportsQuery([FromBody] Report.Application.Features.CQRS.Reports.Query.GetAll.GetAllReportsQueryRequest request)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return Unauthorized();
        }

        var reportingUserIds = await userManager.Users
            .Where(item => item.CompanyId == currentUser.CompanyId)
            .Select(item => item.Id)
            .ToListAsync();

        var scopedRequest = request with { ReportingUserIds = reportingUserIds };
        var result = await mediator.Send(scopedRequest);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetDepartmentReportsQueryRequest")]
    public async Task<IActionResult> GetDepartmentReportsQuery([FromBody] Report.Application.Features.CQRS.Reports.Query.GetByDepartment.GetDepartmentReportsQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetReportByIdQueryRequest")]
    public async Task<IActionResult> GetReportByIdQuery([FromBody] Report.Application.Features.CQRS.Reports.Query.GetById.GetReportByIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("UpdateReportCommandRequest")]
    public async Task<IActionResult> UpdateReportCommand([FromBody] Report.Application.Features.CQRS.Reports.Command.Update.UpdateReportCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}
