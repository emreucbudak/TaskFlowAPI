using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateSubTask
{
    public class UpdateSubTaskCommandHandler : IRequestHandler<UpdateSubTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public UpdateSubTaskCommandHandler(
            IProjectManagementReadRepository repository,
            ICapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(UpdateSubTaskCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var parentTask = await _repository.GetTask(request.TaskId, true);

                if (parentTask == null)
                {
                    throw new Exception("Ana görev bulunamadı.");
                }

                var subTask = parentTask.GetSubtask(request.SubTasksId);

                if (subTask == null)
                {
                    throw new Exception("Alt görev bulunamadı.");
                }

                subTask.UpdateTaskTitle(request.TaskTitle);
                subTask.UpdateTaskDescription(request.Description);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("SubTaskUpdated", new SubTaskUpdatedIntegrationEvent(
                    request.TaskId,
                    request.SubTasksId,
                    request.TaskTitle,
                    request.Description,
                    request.ReceiverUserId
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}