using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Update
{
    public class UpdateSubTaskAnswerCommandHandler : IRequestHandler<UpdateSubTaskAnswerCommandRequest>
    {
        private readonly IProjectManagementReadRepository readRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateSubTaskAnswerCommandHandler(IProjectManagementReadRepository readRepository, IUnitOfWork unitOfWork)
        {
            this.readRepository = readRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateSubTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await readRepository.GetTask(request.TaskId, true);
            var subTask = task.GetSubtask(request.SubTaskId);
            subTask.UpdateSubTaskAnswer(request.SubTaskAnswer, request.SubTaskAnswerId);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
