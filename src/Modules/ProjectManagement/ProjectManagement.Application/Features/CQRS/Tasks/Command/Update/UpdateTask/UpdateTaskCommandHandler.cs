using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateTaskCommandHandler(IProjectManagementReadRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var getTask = await _repository.GetTask(request.TaskId, true);
            getTask.UpdateTaskName(request.TaskName);
            getTask.UpdateTaskDescription(request.Description);
            getTask.UpdateDeadlineTime(request.DeadlineTime);
            await unitOfWork.SaveChangesAsync();
            

        }
    }
}
