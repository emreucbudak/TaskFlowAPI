using FlashMediator;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Command.Create
{
    public class CreateTaskAnswerCommandHandler : IRequestHandler<CreateTaskAnswerCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly IUnitOfWork unitOfWork;

        public CreateTaskAnswerCommandHandler(IProjectManagementReadRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetTask(request.TaskId, true);
            task.AddAnswer(request.AnswerText, request.SenderId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
