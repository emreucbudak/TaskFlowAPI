using FlashMediator;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Create
{
    public class CreateSubTasksCommandHandler : IRequestHandler<CreateSubTasksCommandRequest>
    {
    
        private readonly IProjectManagementReadRepository _projectManagementReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSubTasksCommandHandler(IProjectManagementReadRepository projectManagementReadRepository, IUnitOfWork unitOfWork)
        {

            _projectManagementReadRepository = projectManagementReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateSubTasksCommandRequest request, CancellationToken cancellationToken)
        {
            var subTask = await _projectManagementReadRepository.GetTask(request.TaskId, true);
            subTask.AddSubTask(description:request.Description,AssignedUserId:request.AssignedUserId,Title:request.TaskTitle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


        }
    }
}
