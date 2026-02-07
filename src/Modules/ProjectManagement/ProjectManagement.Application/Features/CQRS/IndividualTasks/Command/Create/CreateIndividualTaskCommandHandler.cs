using FlashMediator;
using ProjectManagement.Application.Messaging;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create
{
    public class CreateIndividualTaskCommandHandler : IRequestHandler<CreateIndividualTaskCommandRequest>
    {
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectManagementProducer _producer;

        public CreateIndividualTaskCommandHandler(IProjectManagementWriteRepository writeRepository, IUnitOfWork unitOfWork, IProjectManagementProducer producer)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _producer = producer;
        }

        public async System.Threading.Tasks.Task Handle(CreateIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var task = new Domain.Entities.IndividualTasks(request.AssignedUserId, request.TaskTitle, request.Description, request.Deadline);
            await _writeRepository.AddIndividualTask(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _producer.PublishAsync("IndividualTaskCreated", new
            {
                Id = task.Id,
                AssignedUserId = task.AssignedUserId,
                TaskTitle = task.TaskTitle
            });
        }
    }
}
