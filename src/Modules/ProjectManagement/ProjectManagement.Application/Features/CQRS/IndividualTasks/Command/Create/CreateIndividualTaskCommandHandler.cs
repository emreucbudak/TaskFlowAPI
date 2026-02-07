using FlashMediator;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create
{
    public class CreateIndividualTaskCommandHandler : IRequestHandler<CreateIndividualTaskCommandRequest>
    {
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateIndividualTaskCommandHandler(IProjectManagementWriteRepository writeRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task Handle(CreateIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var task = new Domain.Entities.IndividualTasks(request.AssignedUserId, request.TaskTitle, request.Description, request.Deadline);
            await _writeRepository.AddIndividualTask(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
