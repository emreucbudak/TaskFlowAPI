using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;


namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete
{
    public class DeleteSubTasksCommandHandler : IRequestHandler<DeleteSubTasksCommandRequest>
    {
        private readonly IProjectManagementReadRepository _readRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public DeleteSubTasksCommandHandler(
            IProjectManagementReadRepository readRepository,
            ICapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _readRepository = readRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(DeleteSubTasksCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {

                var task = await _readRepository.GetTask(request.TaskId, true, cancellationToken);

                if (task == null)
                {
                    throw new Exception("Task bulunamadý.");
                }

                task.RemoveSubTask(request.SubTaskId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("SubTaskDeleted", new SubTaskDeletedIntegrationEvent(
                    request.TaskId,
                    request.SubTaskId,
                    request.ReceiverUserId
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
