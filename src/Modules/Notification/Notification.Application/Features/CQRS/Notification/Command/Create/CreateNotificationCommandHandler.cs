using FlashMediator;
using Notification.Application.Repositories;
using Notification.Application.UnitOfWork;

namespace Notification.Application.Features.CQRS.Notification.Command.Create
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommandRequest>
    {
        private readonly INotificationWriteRepository writeRepository;
        private readonly INotificationUnitOfWork unitOfWork;

        public CreateNotificationCommandHandler(INotificationWriteRepository writeRepository, INotificationUnitOfWork unitOfWork)
        {
            this.writeRepository = writeRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateNotificationCommandRequest request, CancellationToken cancellationToken)
        {
            var notificationMessage = new Domain.Models.NotificationMessage(request.Title,request.Description,request.SendTime,request.IsRead,request.ReceiverUserId);
            await writeRepository.SendNotification(notificationMessage);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

