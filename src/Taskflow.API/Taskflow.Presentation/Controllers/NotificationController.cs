using FlashMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotificationController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = "SubscribedCompanyPolicy")]
    [HttpPost("CreateNotificationCommandRequest")]
    public async Task<IActionResult> CreateNotificationCommand([FromBody] Notification.Application.Features.CQRS.Notification.Command.Create.CreateNotificationCommandRequest request)
    {
        await mediator.Send(request);
        return StatusCode(StatusCodes.Status201Created);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("DeleteNotificationCommandRequest")]
    public async Task<IActionResult> DeleteNotificationCommand([FromBody] Notification.Application.Features.CQRS.Notification.Command.Delete.DeleteNotificationCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("GetUserAllNotificationsQueriesRequest")]
    public async Task<IActionResult> GetUserAllNotificationsQueries([FromBody] Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications.GetUserAllNotificationsQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "SubscribedCompanyOrWorkerPolicy")]
    [HttpPost("MarkUserNotificationsAsReadCommandRequest")]
    public async Task<IActionResult> MarkUserNotificationsAsReadCommand([FromBody] Notification.Application.Features.CQRS.Notification.Command.MarkAsRead.MarkUserNotificationsAsReadCommandRequest request)
    {
        var updatedCount = await mediator.Send(request);
        return Ok(updatedCount);
    }
}
