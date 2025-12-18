using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using System.ComponentModel.DataAnnotations;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Update.UpdateTaskStatus
{
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateTaskStatusCommandHandler(IProjectManagementReadRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateTaskStatusCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetTask(request.TaskId, true);
            task.UpdateTaskStatus(request.TaskStatusId);
            await unitOfWork.SaveChangesAsync();

        }
    }
}
