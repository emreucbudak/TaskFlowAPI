using FlashMediator;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Application.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly IProjectManagementCapUnitOfWork unitOfWork;

        public UpdateTaskCommandHandler(IProjectManagementReadRepository repository, IProjectManagementCapUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var getTask = await _repository.GetTask(request.TaskId, true, cancellationToken);
            getTask.UpdateTaskName(request.TaskName);
            getTask.UpdateTaskDescription(request.Description);
            getTask.UpdateDeadlineTime(DateOnly.FromDateTime(DateTime.UtcNow));
            await unitOfWork.SaveChangesAsync(cancellationToken);
            

        }
    }
}


