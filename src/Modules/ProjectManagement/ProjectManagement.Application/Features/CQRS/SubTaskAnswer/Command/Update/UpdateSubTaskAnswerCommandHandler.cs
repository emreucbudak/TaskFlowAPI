using FlashMediator;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Application.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Update
{
    public class UpdateSubTaskAnswerCommandHandler : IRequestHandler<UpdateSubTaskAnswerCommandRequest>
    {
        private readonly IProjectManagementReadRepository readRepository;
        private readonly IProjectManagementCapUnitOfWork unitOfWork;

        public UpdateSubTaskAnswerCommandHandler(IProjectManagementReadRepository readRepository, IProjectManagementCapUnitOfWork unitOfWork)
        {
            this.readRepository = readRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateSubTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await readRepository.GetTask(request.TaskId, true, cancellationToken);
            var subTask = task.GetSubtask(request.SubTaskId);
            subTask.UpdateSubTaskAnswer(request.SubTaskAnswer, request.SubTaskAnswerId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}


