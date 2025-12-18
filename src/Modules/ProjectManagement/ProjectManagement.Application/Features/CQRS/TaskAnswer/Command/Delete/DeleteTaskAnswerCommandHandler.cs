using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;


namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Command.Delete
{
    public class DeleteTaskAnswerCommandHandler : IRequestHandler<DeleteTaskAnswerCommandRequest>
    {
        private readonly IProjectManagementReadRepository readRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteTaskAnswerCommandHandler(IProjectManagementReadRepository readRepository, IUnitOfWork unitOfWork)
        {
            this.readRepository = readRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await readRepository.GetTask(request.TaskId,true);
            task.RemoveTaskAnswer(request.TaskAnswerId);
            await unitOfWork.SaveChangesAsync();

        }
    }
}
