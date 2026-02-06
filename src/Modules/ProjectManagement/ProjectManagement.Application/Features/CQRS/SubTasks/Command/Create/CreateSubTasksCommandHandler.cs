using FlashMediator;
using ProjectManagement.Application.Messaging;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Create
{
    public class CreateSubTasksCommandHandler : IRequestHandler<CreateSubTasksCommandRequest>
    {
    
        private readonly IProjectManagementReadRepository _projectManagementReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectManagementProducer _projectManagementProducer;

        public CreateSubTasksCommandHandler(IProjectManagementReadRepository projectManagementReadRepository, IUnitOfWork unitOfWork, IProjectManagementProducer projectManagementProducer)
        {

            _projectManagementReadRepository = projectManagementReadRepository;
            _unitOfWork = unitOfWork;
            _projectManagementProducer = projectManagementProducer;
        }

        public async Task Handle(CreateSubTasksCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await _projectManagementReadRepository.GetTask(request.TaskId, true);
            task.AddSubTask(description:request.Description,AssignedUserId:request.AssignedUserId,Title:request.TaskTitle,taskId:request.TaskId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _projectManagementProducer.PublishAsync("SubTaskCreated", new
            {
                TaskId = request.TaskId,
                Description = request.Description,
                AssignedUserId = request.AssignedUserId,
                TaskTitle = request.TaskTitle
            });
        }
    }
}
