using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Create
{
    public class CreateSubTasksCommandHandler : IRequestHandler<CreateSubTasksCommandRequest>
    {
        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public CreateSubTasksCommandHandler(
            IProjectManagementReadRepository readRepository,
            IProjectManagementWriteRepository writeRepository,
            ICapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(CreateSubTasksCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var task = await _readRepository.GetTask(request.TaskId, true, cancellationToken);

                if (task == null)
                {

                    throw new Exception("Task bulunamadý.");
                }

                var subTask = task.AddSubTask(
                    description: request.Description,
                    AssignedUserId: request.AssignedUserId,
                    Title: request.TaskTitle,
                    taskId: request.TaskId
                );

                await _writeRepository.AddSubTask(subTask, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("SubTaskCreated", new SubTaskCreatedIntegrationEvent(
                    request.TaskId,
                    request.TaskTitle,
                    request.Description,
                    request.AssignedUserId
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}

