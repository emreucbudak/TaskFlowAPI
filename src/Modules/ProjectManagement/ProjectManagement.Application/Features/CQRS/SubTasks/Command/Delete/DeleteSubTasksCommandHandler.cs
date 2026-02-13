using FlashMediator;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Command.Delete
{
    public class DeleteSubTasksCommandHandler : IRequestHandler<DeleteSubTasksCommandRequest>
    {

        private readonly IProjectManagementReadRepository projectManagementReadRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IProjectManagementProducer _producer;

        public DeleteSubTasksCommandHandler(IProjectManagementReadRepository projectManagementReadRepository, IUnitOfWork unitOfWork, IProjectManagementProducer producer)
        {
            this.projectManagementReadRepository = projectManagementReadRepository;
            this.unitOfWork = unitOfWork;
            _producer = producer;
        }

        public async Task Handle(DeleteSubTasksCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await projectManagementReadRepository.GetTask(request.TaskId, false);
            task.RemoveSubTask(request.SubTaskId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await _producer.PublishAsync("SubTaskDeleted", new
            {
                TaskId = request.TaskId,
                SubTaskId = request.SubTaskId
            });
        }
    }
}
