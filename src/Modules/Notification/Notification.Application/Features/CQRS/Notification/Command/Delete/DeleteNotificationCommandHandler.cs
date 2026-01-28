using FlashMediator;
using Notification.Application.Features.CQRS.Notification.Exceptions;
using Notification.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Notification.Application.Features.CQRS.Notification.Command.Delete
{
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommandRequest>
    {
        private readonly INotificationWriteRepository writeRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly INotificationReadRepository readRepository;

        public DeleteNotificationCommandHandler(INotificationWriteRepository writeRepository, IUnitOfWork unitOfWork, INotificationReadRepository readRepository)
        {
            this.writeRepository = writeRepository;
            this.unitOfWork = unitOfWork;
            this.readRepository = readRepository;
        }

        public async Task Handle(DeleteNotificationCommandRequest request, CancellationToken cancellationToken)
        {
            var notification = await readRepository.GetByIdAsync(false,request.userId,request.notificationId);
            if (notification == null)
            {
                throw new NotificationNotFoundExceptions();
            }
             writeRepository.DeleteNotification(notification);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
