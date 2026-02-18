using FlashMediator;
using Microsoft.AspNetCore.Mvc;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IdentityController(IMediator mediator) : ControllerBase
{
    [HttpPost("AddDepartmentCommandRequest")]
    public async Task<IActionResult> AddDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.Add.AddDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("AddGroupsCommandRequest")]
    public async Task<IActionResult> AddGroupsCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.Add.AddGroupsCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("AddGroupsMemberCommandRequest")]
    public async Task<IActionResult> AddGroupsMemberCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.AddGroupsMember.AddGroupsMemberCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("AddUserToDepartmentCommandRequest")]
    public async Task<IActionResult> AddUserToDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.AddUserToDepartment.AddUserToDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("CreateCompanyCommandRequest")]
    public async Task<IActionResult> CreateCompanyCommand([FromBody] Identity.Application.Features.CQRS.Company.Command.Create.CreateCompanyCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteCompanyCommandRequest")]
    public async Task<IActionResult> DeleteCompanyCommand([FromBody] Identity.Application.Features.CQRS.Company.Command.Delete.DeleteCompanyCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteDepartmentCommandRequest")]
    public async Task<IActionResult> DeleteDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.Delete.DeleteDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteGroupsCommandRequest")]
    public async Task<IActionResult> DeleteGroupsCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.Delete.DeleteGroupsCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteGroupsMemberCommandRequest")]
    public async Task<IActionResult> DeleteGroupsMemberCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.DeleteGroupsMember.DeleteGroupsMemberCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteUserFromDepartmentCommandRequest")]
    public async Task<IActionResult> DeleteUserFromDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.DeleteUserFromDepartment.DeleteUserFromDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("GetAllCompaniesQueriesRequest")]
    public async Task<IActionResult> GetAllCompaniesQueries([FromBody] Identity.Application.Features.CQRS.Company.Queries.GetAll.GetAllCompaniesQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("GetAllCompanyGroupsQueriesRequest")]
    public async Task<IActionResult> GetAllCompanyGroupsQueries([FromBody] Identity.Application.Features.CQRS.Groups.Queries.GetAll.GetAllCompanyGroupsQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("GetDepartmentLeaderQueryRequest")]
    public async Task<IActionResult> GetDepartmentLeaderQuery([FromBody] Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader.GetDepartmentLeaderQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("LoginCommandRequest")]
    public async Task<IActionResult> LoginCommand([FromBody] Identity.Application.Features.CQRS.Auth.Login.LoginCommandRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("RegisterCommandRequest")]
    public async Task<IActionResult> RegisterCommand([FromBody] Identity.Application.Features.CQRS.Auth.Register.RegisterCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateCompanyCommandRequest")]
    public async Task<IActionResult> UpdateCompanyCommand([FromBody] Identity.Application.Features.CQRS.Company.Command.Update.UpdateCompanyCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateDepartmentCommandRequest")]
    public async Task<IActionResult> UpdateDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.Update.UpdateDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateGroupsCommandRequest")]
    public async Task<IActionResult> UpdateGroupsCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.Update.UpdateGroupsCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}

