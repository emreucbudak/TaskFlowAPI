using FlashMediator;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Application.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Delete
{
    public class DeleteSubTaskAnswerCommandHandler : IRequestHandler<DeleteSubTaskAnswerCommandRequest, bool>
    {
        private readonly IProjectManagementReadRepository readRepository;
        private readonly IProjectManagementCapUnitOfWork unitOfWork;

        public DeleteSubTaskAnswerCommandHandler(IProjectManagementReadRepository readRepository, IProjectManagementCapUnitOfWork unitOfWork)
        {
            this.readRepository = readRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSubTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await readRepository.GetTask(request.TaskId, true, cancellationToken);
            var subTask = task.GetSubtask(request.SubTaskId);
            subTask.RemoveSubTaskAnswer(request.SubTaskAnswerId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}


