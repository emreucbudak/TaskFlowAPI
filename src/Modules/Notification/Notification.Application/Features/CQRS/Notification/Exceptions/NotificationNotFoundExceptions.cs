using TaskFlow.BuildingBlocks.Exceptions;

namespace Notification.Application.Features.CQRS.Notification.Exceptions
{
    public class NotificationNotFoundExceptions : NotFoundExceptions
    {
        public NotificationNotFoundExceptions() : base("Bildirim bulunamadı!")
        {
        }
    }
}
