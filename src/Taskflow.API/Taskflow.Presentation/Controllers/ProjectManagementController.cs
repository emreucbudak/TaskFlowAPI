using FlashMediator;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Features.CQRS.Tasks.Queries.GetByAssignedUsers;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProjectManagementController(IMediator mediator, UserManager<User> userManager) : ControllerBase
{
    private const int DepartmentLeaderRoleId = 1;
    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("CreateIndividualTaskCommandRequest")]
    public async Task<IActionResult> CreateIndividualTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create.CreateIndividualTaskCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return Unauthorized();
        }

        if (currentUser.CompanyId != request.CompanyId)
        {
            return Forbid();
        }

        var actorRoles = await userManager.GetRolesAsync(currentUser);
        var isCompanyUser = actorRoles.Any(role => string.Equals(role, "Company", StringComparison.OrdinalIgnoreCase));

        if (!isCompanyUser)
        {
            var managedDepartmentIds = await userManager.Users
                .AsNoTracking()
                .Where(user => user.Id == currentUser.Id && user.CompanyId == request.CompanyId)
                .SelectMany(user => user.DepartmentMembers
                    .Where(member => member.DepartmentRoleId == DepartmentLeaderRoleId)
                    .Select(member => member.DepartmentId))
                .Distinct()
                .ToListAsync(cancellationToken);

            if (managedDepartmentIds.Count == 0)
            {
                return Forbid();
            }

            var canAssignUser = await userManager.Users
                .AsNoTracking()
                .Where(user => user.Id == request.AssignedUserId && user.CompanyId == request.CompanyId)
                .AnyAsync(user => user.DepartmentMembers.Any(member => managedDepartmentIds.Contains(member.DepartmentId)), cancellationToken);

            if (!canAssignUser)
            {
                return Forbid();
            }
        }

        await mediator.Send(request, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "SubscribedWorkerPolicy")]
    [HttpPost("CreateSubTaskAnswerCommandRequest")]
    public async Task<IActionResult> CreateSubTaskAnswerCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Create.CreateSubTaskAnswerCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("CreateSubTasksCommandRequest")]
    public async Task<IActionResult> CreateSubTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Create.CreateSubTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("CreateTasksCommandRequest")]
    public async Task<IActionResult> CreateTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Create.CreateTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("CreateGroupTaskWithSubTasksCommandRequest")]
    public async Task<IActionResult> CreateGroupTaskWithSubTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.CreateGroupTask.CreateGroupTaskWithSubTasksCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedWorkerPolicy")]
    [HttpPost("CompleteIndividualTaskCommandRequest")]
    public async Task<IActionResult> CompleteIndividualTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Complete.CompleteIndividualTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteIndividualTaskCommandRequest")]
    public async Task<IActionResult> DeleteIndividualTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Delete.DeleteIndividualTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedWorkerPolicy")]
    [HttpPost("DeleteSubTaskAnswerCommandRequest")]
    public async Task<IActionResult> DeleteSubTaskAnswerCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Delete.DeleteSubTaskAnswerCommandRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteSubTasksCommandRequest")]
    public async Task<IActionResult> DeleteSubTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete.DeleteSubTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("DeleteTasksCommandRequest")]
    public async Task<IActionResult> DeleteTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Delete.DeleteTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetAllSubTaskAnswerQueriesRequest")]
    public async Task<IActionResult> GetAllSubTaskAnswerQueries([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Queries.GetAll.GetAllSubTaskAnswerQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetAllSubTasksQueriesRequest")]
    public async Task<IActionResult> GetAllSubTasksQueries([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Queries.GetAll.GetAllSubTasksQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetAllTasksQueriesRequest")]
    public async Task<IActionResult> GetAllTasksQueries([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Queries.GetAllTasksQueriesRequest request)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return Unauthorized();
        }

        var assignedUserIds = await userManager.Users
            .Where(item => item.CompanyId == currentUser.CompanyId)
            .Select(item => item.Id)
            .ToListAsync();

        var scopedRequest = new GetGroupTasksByAssignedUsersQueryRequest
        {
            AssignedUserIds = assignedUserIds,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var result = await mediator.Send(scopedRequest);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetGroupTasksByAssignedUsersQueryRequest")]
    public async Task<IActionResult> GetGroupTasksByAssignedUsersQuery([FromBody] GetGroupTasksByAssignedUsersQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetIndividualTaskByIdQueryRequest")]
    public async Task<IActionResult> GetIndividualTaskByIdQuery([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetById.GetIndividualTaskByIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetIndividualTasksByUserIdQueryRequest")]
    public async Task<IActionResult> GetIndividualTasksByUserIdQuery([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId.GetIndividualTasksByUserIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("UpdateIndividualTaskCommandRequest")]
    public async Task<IActionResult> UpdateIndividualTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Update.UpdateIndividualTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedWorkerPolicy")]
    [HttpPost("UpdateSubTaskAnswerCommandRequest")]
    public async Task<IActionResult> UpdateSubTaskAnswerCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Update.UpdateSubTaskAnswerCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("UpdateSubTaskCommandRequest")]
    public async Task<IActionResult> UpdateSubTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateSubTask.UpdateSubTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedWorkerPolicy")]
    [HttpPost("UpdateSubTasksStatusCommandRequest")]
    public async Task<IActionResult> UpdateSubTasksStatusCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateStatus.UpdateSubTasksStatusCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("UpdateTaskCommandRequest")]
    public async Task<IActionResult> UpdateTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTask.UpdateTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedWorkerPolicy")]
    [HttpPost("UpdateTaskStatusCommandRequest")]
    public async Task<IActionResult> UpdateTaskStatusCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTaskStatus.UpdateTaskStatusCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}

