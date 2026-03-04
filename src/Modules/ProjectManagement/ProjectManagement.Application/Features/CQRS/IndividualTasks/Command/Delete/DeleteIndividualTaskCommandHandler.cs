using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Delete
{
    public class DeleteIndividualTaskCommandHandler : IRequestHandler<DeleteIndividualTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public DeleteIndividualTaskCommandHandler(
            IProjectManagementReadRepository readRepository,
            IProjectManagementWriteRepository writeRepository,
            ICapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(DeleteIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var task = await _readRepository.GetIndividualTask(request.Id, false);
                if (task == null)
                {
                    throw new IndividualTaskNotFoundException();
                }

                await _writeRepository.DeleteIndividualTask(task);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("IndividualTaskDeleted", new IndividualTaskDeletedIntegrationEvent(
                    task.Id,
                    task.AssignedUserId
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
