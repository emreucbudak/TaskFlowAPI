using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete
{
    public class DeleteSubTasksCommandHandler : IRequestHandler<DeleteSubTasksCommandRequest>
    {

        private readonly IProjectManagementReadRepository projectManagementReadRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteSubTasksCommandHandler(IProjectManagementReadRepository projectManagementReadRepository, IUnitOfWork unitOfWork)
        {
            this.projectManagementReadRepository = projectManagementReadRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteSubTasksCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await projectManagementReadRepository.GetTask(request.TaskId, false);
             task.RemoveSubTask(request.TaskId);
            await unitOfWork.SaveChangesAsync();

            
        }
    }
}
