using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Create
{
    public class CreateTasksCommandHandler : IRequestHandler<CreateTasksCommandRequest>
    {
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateTasksCommandHandler(IUnitOfWork unitOfWork, IProjectManagementWriteRepository writeRepository)
        {
            this.unitOfWork = unitOfWork;
            _writeRepository = writeRepository;
        }

        public async Task Handle(CreateTasksCommandRequest request, CancellationToken cancellationToken)
        {
            var task = new Domain.Entities.Task(request.TaskName, request.Description, request.DeadlineTime);
            await _writeRepository.AddTask(task);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
