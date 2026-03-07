using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Application.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateStatus
{
    public class UpdateSubTasksStatusCommandHandler : IRequestHandler<UpdateSubTasksStatusCommandRequest>
    {
        private const int CompletedStatusId = 4;
        private const string GroupTaskCompletedTopic = "GroupTaskCompleted";

        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementCapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public UpdateSubTasksStatusCommandHandler(
            IProjectManagementReadRepository readRepository,
            IProjectManagementCapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _readRepository = readRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(UpdateSubTasksStatusCommandRequest request, CancellationToken cancellationToken)
        {
            using var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false);

            var task = await _readRepository.GetTask(request.TasksId, true, cancellationToken);
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


