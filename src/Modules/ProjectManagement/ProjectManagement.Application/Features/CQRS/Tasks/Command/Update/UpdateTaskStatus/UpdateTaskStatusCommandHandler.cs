using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTaskStatus
{
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommandRequest>
    {
        private const int CompletedStatusId = 4;
        private const string GroupTaskCompletedTopic = "GroupTaskCompleted";

        private readonly IProjectManagementReadRepository _repository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public UpdateTaskStatusCommandHandler(
            IProjectManagementReadRepository repository,
            ICapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(UpdateTaskStatusCommandRequest request, CancellationToken cancellationToken)
        {
            using var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false);

            var task = await _repository.GetTask(request.TaskId, true);
            var wasCompleted = task.TaskStatusId == CompletedStatusId;

            task.UpdateTaskStatus(request.TaskStatusId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!wasCompleted && request.TaskStatusId == CompletedStatusId)
            {
                var assignedUserIds = task
                    .GetAllSubTasks()
                    .Select(subTask => subTask.AssignedUserId)
                    .Where(userId => userId != Guid.Empty)
                    .Distinct()
                    .ToList();

                if (assignedUserIds.Count > 0)
                {
                    var completedOn = DateOnly.FromDateTime(DateTime.UtcNow);
                    await _capPublisher.PublishAsync(
                        GroupTaskCompletedTopic,
                        new GroupTaskCompletedIntegrationEvent(
                            task.Id,
                            task.DeadlineTime,
                            completedOn,
                            assignedUserIds));
                }
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
