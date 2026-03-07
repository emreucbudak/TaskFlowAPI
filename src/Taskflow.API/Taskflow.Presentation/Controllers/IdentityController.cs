using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class IdentityController(IMediator mediator, UserManager<User> userManager) : ControllerBase
{
    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("AddDepartmentCommandRequest")]
    public async Task<IActionResult> AddDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.Add.AddDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("AddGroupsCommandRequest")]
    public async Task<IActionResult> AddGroupsCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.Add.AddGroupsCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("AddGroupsMemberCommandRequest")]
    public async Task<IActionResult> AddGroupsMemberCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.AddGroupsMember.AddGroupsMemberCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("AddUserToDepartmentCommandRequest")]
    public async Task<IActionResult> AddUserToDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.AddUserToDepartment.AddUserToDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("CreateCompanyCommandRequest")]
    public async Task<IActionResult> CreateCompanyCommand([FromBody] Identity.Application.Features.CQRS.Company.Command.Create.CreateCompanyCommandRequest request)
    {
        var companyId = await mediator.Send(request);
        return Ok(new { companyId, request.CompanyName });
    }

    [Authorize(Policy = "AdminOrCompanyPolicy")]
    [HttpPost("DeleteCompanyCommandRequest")]
    public async Task<IActionResult> DeleteCompanyCommand([FromBody] Identity.Application.Features.CQRS.Company.Command.Delete.DeleteCompanyCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteDepartmentCommandRequest")]
    public async Task<IActionResult> DeleteDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.Delete.DeleteDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteGroupsCommandRequest")]
    public async Task<IActionResult> DeleteGroupsCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.Delete.DeleteGroupsCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteGroupsMemberCommandRequest")]
    public async Task<IActionResult> DeleteGroupsMemberCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.DeleteGroupsMember.DeleteGroupsMemberCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteUserFromDepartmentCommandRequest")]
    public async Task<IActionResult> DeleteUserFromDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.DeleteUserFromDepartment.DeleteUserFromDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteWorkerCommandRequest")]
    public async Task<IActionResult> DeleteWorkerCommand([FromBody] Identity.Application.Features.CQRS.Auth.Command.DeleteWorker.DeleteWorkerCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("ChangeUserPasswordCommandRequest")]
    public async Task<IActionResult> ChangeUserPasswordCommand([FromBody] Identity.Application.Features.CQRS.Auth.Command.ChangeUserPassword.ChangeUserPasswordCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost("GetAllCompaniesQueriesRequest")]
    public async Task<IActionResult> GetAllCompaniesQueries([FromBody] Identity.Application.Features.CQRS.Company.Queries.GetAll.GetAllCompaniesQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetAllCompanyGroupsQueriesRequest")]
    public async Task<IActionResult> GetAllCompanyGroupsQueries([FromBody] Identity.Application.Features.CQRS.Groups.Queries.GetAll.GetAllCompanyGroupsQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetDepartmentLeaderQueryRequest")]
    public async Task<IActionResult> GetDepartmentLeaderQuery([FromBody] Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader.GetDepartmentLeaderQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetAllCompanyUsersQueriesRequest")]
    public async Task<IActionResult> GetAllCompanyUsersQueries([FromBody] Identity.Application.Features.CQRS.Auth.Queries.GetAllCompanyUsers.GetAllCompanyUsersQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("SearchCompanyUsersQueryRequest")]
    public async Task<IActionResult> SearchCompanyUsersQuery([FromBody] Identity.Application.Features.CQRS.Auth.Queries.SearchCompanyUsers.SearchCompanyUsersQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetAllCompanyDepartmentsQueriesRequest")]
    public async Task<IActionResult> GetAllCompanyDepartmentsQueries([FromBody] Identity.Application.Features.CQRS.Department.Query.GetAll.GetAllCompanyDepartmentsQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("LoginCommandRequest")]
    public async Task<IActionResult> LoginCommand([FromBody] Identity.Application.Features.CQRS.Auth.Login.LoginCommandRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "CompanyOrWorkerPolicy")]
    [HttpGet("GetCurrentUserContext")]
    public async Task<IActionResult> GetCurrentUserContext()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        return Ok(new
        {
            UserId = user.Id,
            CompanyId = user.CompanyId,
            Role = roles.FirstOrDefault() ?? string.Empty
        });
    }

    [AllowAnonymous]
    [HttpPost("RegisterCommandRequest")]
    public async Task<IActionResult> RegisterCommand([FromBody] Identity.Application.Features.CQRS.Auth.Register.RegisterCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "AdminOrCompanyPolicy")]
    [HttpPost("UpdateCompanyCommandRequest")]
    public async Task<IActionResult> UpdateCompanyCommand([FromBody] Identity.Application.Features.CQRS.Company.Command.Update.UpdateCompanyCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("UpdateDepartmentCommandRequest")]
    public async Task<IActionResult> UpdateDepartmentCommand([FromBody] Identity.Application.Features.CQRS.Department.Command.Update.UpdateDepartmentCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("UpdateGroupsCommandRequest")]
    public async Task<IActionResult> UpdateGroupsCommand([FromBody] Identity.Application.Features.CQRS.Groups.Command.Update.UpdateGroupsCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}
