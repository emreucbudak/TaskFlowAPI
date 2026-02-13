using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;


namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Update
{
    public class UpdateIndividualTaskCommandHandler : IRequestHandler<UpdateIndividualTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public UpdateIndividualTaskCommandHandler(
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

        public async Task Handle(UpdateIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var task = await _readRepository.GetIndividualTask(request.Id, true);
                if (task == null)
                {
                    throw new IndividualTaskNotFoundException();
                }

                task.Update(request.TaskTitle, request.Description, request.Deadline);

                await _writeRepository.UpdateIndividualTask(task);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("IndividualTaskUpdated", new IndividualTaskUpdatedIntegrationEvent(
                    task.Id,
                    task.AssignedUserId,
                    task.TaskTitle,
                    task.Description
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}