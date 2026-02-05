using FlashMediator;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Domain.Entities;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Delete
{
    public class DeleteSubTaskAnswerCommandHandler : IRequestHandler<DeleteSubTaskAnswerCommandRequest, bool>
    {
        private readonly IProjectManagementReadRepository readRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteSubTaskAnswerCommandHandler(IProjectManagementReadRepository readRepository, IUnitOfWork unitOfWork)
        {
            this.readRepository = readRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSubTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await readRepository.GetTask(request.TaskId, true);
            var subTask = task.GetSubtask(request.SubTaskId);
            subTask.RemoveSubTaskAnswer(request.SubTaskAnswerId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
