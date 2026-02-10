using FlashMediator;
using ProjectManagement.Application.Messaging;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateSubTask
{
    public class UpdateSubTaskCommandHandler : IRequestHandler<UpdateSubTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository  _repository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IProjectManagementProducer _producer;

        public UpdateSubTaskCommandHandler(IProjectManagementReadRepository repository, IUnitOfWork unitOfWork, IProjectManagementProducer producer)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
            _producer = producer;
        }

        public async Task Handle(UpdateSubTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var getSubTask = await _repository.GetTask(request.TaskId, true);
            var task = getSubTask.GetSubtask(request.SubTasksId);
            task.UpdateTaskTitle(request.TaskTitle);
            task.UpdateTaskDescription(request.Description);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await _producer.PublishAsync("SubTaskUpdated", new
            {
                TaskId = request.TaskId,
                SubTaskId = request.SubTasksId,
                TaskTitle = request.TaskTitle,
                Description = request.Description
            });
        }
    }
}
