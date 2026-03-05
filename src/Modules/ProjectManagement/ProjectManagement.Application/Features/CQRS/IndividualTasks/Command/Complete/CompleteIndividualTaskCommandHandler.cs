using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Complete
{
    public class CompleteIndividualTaskCommandHandler : IRequestHandler<CompleteIndividualTaskCommandRequest>
    {
        private const string IndividualTaskCompletedTopic = "IndividualTaskCompleted";

        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public CompleteIndividualTaskCommandHandler(
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

        public async Task Handle(CompleteIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            using var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false);

            var task = await _readRepository.GetIndividualTask(request.Id, false, cancellationToken);
            if (task == null)
            {
                throw new IndividualTaskNotFoundException();
            }

            await _writeRepository.DeleteIndividualTask(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (task.AssignedUserId != Guid.Empty)
            {
                await _capPublisher.PublishAsync(
                    IndividualTaskCompletedTopic,
                    new IndividualTaskCompletedIntegrationEvent(
                        task.Id,
                        task.AssignedUserId,
                        task.Deadline,
                        DateOnly.FromDateTime(DateTime.UtcNow)));
            }

            await transaction.CommitAsync(cancellationToken);
        }
    }
}

