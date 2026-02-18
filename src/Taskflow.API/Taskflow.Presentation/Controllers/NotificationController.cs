using FlashMediator;
using Microsoft.AspNetCore.Mvc;

namespace Taskflow.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotificationController(IMediator mediator) : ControllerBase
{
    [HttpPost("CreateNotificationCommandRequest")]
    public async Task<IActionResult> CreateNotificationCommand([FromBody] Notification.Application.Features.CQRS.Notification.Command.Create.CreateNotificationCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("DeleteNotificationCommandRequest")]
    public async Task<IActionResult> DeleteNotificationCommand([FromBody] Notification.Application.Features.CQRS.Notification.Command.Delete.DeleteNotificationCommandRequest request)
    {
        await mediator.Send(request);
        return Ok();
    }

    [HttpPost("GetUserAllNotificationsQueriesRequest")]
    public async Task<IActionResult> GetUserAllNotificationsQueries([FromBody] Notification.Application.Features.CQRS.Notification.Queries.GetAllNotifications.GetUserAllNotificationsQueriesRequest request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }
}

