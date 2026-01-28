using FlashMediator;
using Notification.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Notification.Application.Features.CQRS.Notification.Command.Create
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommandRequest>
    {
        private readonly INotificationWriteRepository writeRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateNotificationCommandHandler(INotificationWriteRepository writeRepository, IUnitOfWork unitOfWork)
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
