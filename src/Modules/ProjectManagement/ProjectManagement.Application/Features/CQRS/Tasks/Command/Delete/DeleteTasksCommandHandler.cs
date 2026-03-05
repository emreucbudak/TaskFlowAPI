using FlashMediator;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Delete
{
    public class DeleteTasksCommandHandler : IRequestHandler<DeleteTasksCommandRequest>
    {
        private readonly IProjectManagementReadRepository readRepository;
        private readonly IProjectManagementWriteRepository projectManagementWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTasksCommandHandler(IProjectManagementReadRepository readRepository, IUnitOfWork unitOfWork, IProjectManagementWriteRepository projectManagementWriteRepository)
        {
            this.readRepository = readRepository;
            _unitOfWork = unitOfWork;
            this.projectManagementWriteRepository = projectManagementWriteRepository;
        }

        public async Task Handle(DeleteTasksCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await readRepository.GetTask(request.Id, false, cancellationToken);
            await projectManagementWriteRepository.DeleteTask(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}


