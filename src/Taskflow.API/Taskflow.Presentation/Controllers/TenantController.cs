using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TenantController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("CompanyPlans")]
    public async Task<IActionResult> GetCompanyPlans()
    {
        var result = await mediator.Send(new Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll.GetAllCompanyPlanQueriesRequest());
        return Ok(result);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("CreateCompanyPlanCommandRequest")]
    public async Task<IActionResult> CreateCompanyPlanCommand([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Command.Create.CreateCompanyPlanCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("DeleteCompanyPlanCommandRequest")]
    public async Task<IActionResult> DeleteCompanyPlanCommand([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete.DeleteCompanyPlanCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("GetAllCompanyPlanQueriesRequest")]
    public async Task<IActionResult> GetAllCompanyPlanQueries([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Queries.GetAll.GetAllCompanyPlanQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("UpdateCompanyPlanCommandRequest")]
    public async Task<IActionResult> UpdateCompanyPlanCommand([FromBody] Tenant.Application.Features.CQRS.CompanyPlan.Command.Update.UpdateCompanyPlanCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}

