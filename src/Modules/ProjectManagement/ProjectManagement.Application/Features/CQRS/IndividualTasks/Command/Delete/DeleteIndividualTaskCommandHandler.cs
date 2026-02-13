using FlashMediator;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Delete
{
    public class DeleteIndividualTaskCommandHandler : IRequestHandler<DeleteIndividualTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectManagementProducer _producer;

        public DeleteIndividualTaskCommandHandler(IProjectManagementReadRepository readRepository, IProjectManagementWriteRepository writeRepository, IUnitOfWork unitOfWork, IProjectManagementProducer producer)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _producer = producer;
        }

        public async System.Threading.Tasks.Task Handle(DeleteIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await _readRepository.GetIndividualTask(request.Id, false);
            if (task == null)
            {
                throw new IndividualTaskNotFoundException();
            }

            await _writeRepository.DeleteIndividualTask(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _producer.PublishAsync("IndividualTaskDeleted", new
            {
                Id = task.Id,
                AssignedUserId = task.AssignedUserId
            });
        }
    }
}