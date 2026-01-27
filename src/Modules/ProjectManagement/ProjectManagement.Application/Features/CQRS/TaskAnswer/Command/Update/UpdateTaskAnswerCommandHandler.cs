using FlashMediator;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Command.Update
{
    public class UpdateTaskAnswerCommandHandler : IRequestHandler<UpdateTaskAnswerCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateTaskAnswerCommandHandler(IProjectManagementReadRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetTask(request.TaskId, true);
            task.UpdateTaskAnswer(request.TaskAnswerId, request.TaskAnswer);
            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
