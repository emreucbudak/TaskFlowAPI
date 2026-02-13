using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create
{
    public class CreateIndividualTaskCommandHandler : IRequestHandler<CreateIndividualTaskCommandRequest>
    {
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public CreateIndividualTaskCommandHandler(IProjectManagementWriteRepository writeRepository, ICapUnitOfWork unitOfWork, ICapPublisher capPublisher)
        {
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(CreateIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var task = new Domain.Entities.IndividualTasks(request.AssignedUserId, request.TaskTitle, request.Description, request.Deadline);

                await _writeRepository.AddIndividualTask(task);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("IndividualTaskCreated", new IndividualTaskCreatedIntegrationEvent(
                    task.AssignedUserId,
                    task.TaskTitle,
                    task.Description,
                    task.Deadline
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}