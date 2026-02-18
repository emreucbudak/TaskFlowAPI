using FlashMediator;
using Microsoft.AspNetCore.Mvc;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProjectManagementController(IMediator mediator) : ControllerBase
{
    [HttpPost("CreateIndividualTaskCommandRequest")]
    public async Task<IActionResult> CreateIndividualTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create.CreateIndividualTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("CreateSubTaskAnswerCommandRequest")]
    public async Task<IActionResult> CreateSubTaskAnswerCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Create.CreateSubTaskAnswerCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("CreateSubTasksCommandRequest")]
    public async Task<IActionResult> CreateSubTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Create.CreateSubTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("CreateTasksCommandRequest")]
    public async Task<IActionResult> CreateTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Create.CreateTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteIndividualTaskCommandRequest")]
    public async Task<IActionResult> DeleteIndividualTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Delete.DeleteIndividualTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteSubTaskAnswerCommandRequest")]
    public async Task<IActionResult> DeleteSubTaskAnswerCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Delete.DeleteSubTaskAnswerCommandRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("DeleteSubTasksCommandRequest")]
    public async Task<IActionResult> DeleteSubTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete.DeleteSubTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteTasksCommandRequest")]
    public async Task<IActionResult> DeleteTasksCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Delete.DeleteTasksCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("GetAllSubTaskAnswerQueriesRequest")]
    public async Task<IActionResult> GetAllSubTaskAnswerQueries([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Queries.GetAll.GetAllSubTaskAnswerQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("GetAllSubTasksQueriesRequest")]
    public async Task<IActionResult> GetAllSubTasksQueries([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Queries.GetAll.GetAllSubTasksQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("GetAllTasksQueriesRequest")]
    public async Task<IActionResult> GetAllTasksQueries([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Queries.GetAllTasksQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("GetIndividualTaskByIdQueryRequest")]
    public async Task<IActionResult> GetIndividualTaskByIdQuery([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetById.GetIndividualTaskByIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("GetIndividualTasksByUserIdQueryRequest")]
    public async Task<IActionResult> GetIndividualTasksByUserIdQuery([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Queries.GetByUserId.GetIndividualTasksByUserIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("UpdateIndividualTaskCommandRequest")]
    public async Task<IActionResult> UpdateIndividualTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Update.UpdateIndividualTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateSubTaskAnswerCommandRequest")]
    public async Task<IActionResult> UpdateSubTaskAnswerCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Update.UpdateSubTaskAnswerCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateSubTaskCommandRequest")]
    public async Task<IActionResult> UpdateSubTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateSubTask.UpdateSubTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateSubTasksStatusCommandRequest")]
    public async Task<IActionResult> UpdateSubTasksStatusCommand([FromBody] ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateStatus.UpdateSubTasksStatusCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateTaskCommandRequest")]
    public async Task<IActionResult> UpdateTaskCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTask.UpdateTaskCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("UpdateTaskStatusCommandRequest")]
    public async Task<IActionResult> UpdateTaskStatusCommand([FromBody] ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTaskStatus.UpdateTaskStatusCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}

