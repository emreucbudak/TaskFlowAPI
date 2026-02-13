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
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public CreateSubTasksCommandHandler(
            IProjectManagementReadRepository readRepository,
            ICapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _readRepository = readRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(CreateSubTasksCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var task = await _readRepository.GetTask(request.TaskId, true);

                if (task == null)
                {

                    throw new Exception("Task bulunamadı.");
                }

                task.AddSubTask(
                    description: request.Description,
                    AssignedUserId: request.AssignedUserId,
                    Title: request.TaskTitle,
                    taskId: request.TaskId
                );

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