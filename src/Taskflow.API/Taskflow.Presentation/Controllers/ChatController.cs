using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ChatController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("CreateMessageCommandRequest")]
    public async Task<IActionResult> CreateMessageCommand([FromBody] Chat.Application.Features.CQRS.Message.Command.Create.CreateMessageCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("DeleteMessageCommandRequest")]
    public async Task<IActionResult> DeleteMessageCommand([FromBody] Chat.Application.Features.CQRS.Message.Command.Delete.DeleteMessageCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetMessagesBetweenUsersQueryRequest")]
    public async Task<IActionResult> GetMessagesBetweenUsersQuery([FromBody] Chat.Application.Features.CQRS.Message.Queries.GetMessagesBetweenUsers.GetMessagesBetweenUsersQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetMessagesByGroupIdQueryRequest")]
    public async Task<IActionResult> GetMessagesByGroupIdQuery([FromBody] Chat.Application.Features.CQRS.Message.Queries.GetMessagesByGroupId.GetMessagesByGroupIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetMessagesByUserIdQueryRequest")]
    public async Task<IActionResult> GetMessagesByUserIdQuery([FromBody] Chat.Application.Features.CQRS.Message.Queries.GetMessagesByUserId.GetMessagesByUserIdQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetUnreadMessageCountQueryRequest")]
    public async Task<IActionResult> GetUnreadMessageCountQuery([FromBody] Chat.Application.Features.CQRS.Message.Queries.GetUnreadMessageCount.GetUnreadMessageCountQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("MarkConversationAsReadCommandRequest")]
    public async Task<IActionResult> MarkConversationAsReadCommand([FromBody] Chat.Application.Features.CQRS.Message.Command.MarkConversationAsRead.MarkConversationAsReadCommandRequest request)
    {
        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(request with { CurrentUserId = currentUserId });
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("MarkAsDeliveredCommandRequest")]
    public async Task<IActionResult> MarkAsDeliveredCommand([FromBody] Chat.Application.Features.CQRS.Message.Command.MarkAsDelivered.MarkAsDeliveredCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("SearchMessagesQueryRequest")]
    public async Task<IActionResult> SearchMessagesQuery([FromBody] Chat.Application.Features.CQRS.Message.Queries.SearchMessages.SearchMessagesQueryRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("UpdateMessageCommandRequest")]
    public async Task<IActionResult> UpdateMessageCommand([FromBody] Chat.Application.Features.CQRS.Message.Command.Update.UpdateMessageCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }
}
