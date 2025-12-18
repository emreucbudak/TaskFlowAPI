using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateSubTask
{
    public class UpdateSubTaskCommandHandler : IRequestHandler<UpdateSubTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateSubTaskCommandHandler(IProjectManagementReadRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateSubTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var getSubTask = await _repository.GetTask(request.TaskId, true);
            var task = getSubTask.GetSubtask(request.SubTasksId);
            task.UpdateTaskTitle(request.TaskTitle);
            task.UpdateTaskDescription(request.Description);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
