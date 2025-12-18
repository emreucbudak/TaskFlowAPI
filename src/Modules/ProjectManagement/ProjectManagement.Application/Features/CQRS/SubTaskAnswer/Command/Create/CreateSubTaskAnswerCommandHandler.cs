using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Domain.Entities;
using System.Security.Cryptography.X509Certificates;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Create
{
    public class CreateSubTaskAnswerCommandHandler : IRequestHandler<CreateSubTaskAnswerCommandRequest>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly IUnitOfWork unitOfWork;
        

        public CreateSubTaskAnswerCommandHandler(IProjectManagementReadRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            this.unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task Handle(CreateSubTaskAnswerCommandRequest request, CancellationToken cancellationToken)
        {
            var Task = await _repository.GetTask(request.TaskId, true);
            var subTask = Task.GetSubtask(request.SubTaskId);
            var answer = new Domain.Entities.SubTaskAnswer(request.AnswerText, request.SenderId);
            subTask.AddAnswer(answer);
            await unitOfWork.SaveChangesAsync();

        }
    }
}
