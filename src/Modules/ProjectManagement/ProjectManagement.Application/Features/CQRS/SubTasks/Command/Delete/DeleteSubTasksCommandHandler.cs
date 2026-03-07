using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Application.UnitOfWork;


namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete
{
    public class DeleteSubTasksCommandHandler : IRequestHandler<DeleteSubTasksCommandRequest>
    {
        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementCapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public DeleteSubTasksCommandHandler(
            IProjectManagementReadRepository readRepository,
            IProjectManagementCapUnitOfWork unitOfWork,
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
                    throw new Exception("Task bulunamadı.");
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

