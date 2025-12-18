using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Update.UpdateStatus
{
    public class UpdateSubTasksStatusCommandHandler : IRequestHandler<UpdateSubTasksStatusCommandRequest>
    {
        private readonly IProjectManagementReadRepository readRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateSubTasksStatusCommandHandler(IProjectManagementReadRepository readRepository, IUnitOfWork unitOfWork)
        {
            this.readRepository = readRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateSubTasksStatusCommandRequest request, CancellationToken cancellationToken)
        {
            var subTask = await readRepository.GetTask(request.TasksId, true);
            subTask.UpdateTaskStatus(request.TaskStatusId);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
