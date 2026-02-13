using FlashMediator;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Update
{
    public class UpdateIndividualTaskCommandHandler : IRequestHandler<UpdateIndividualTaskCommandRequest>
    {
        private readonly IProjectManagementReadRepository _readRepository;
        private readonly IProjectManagementWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectManagementProducer _producer;

        public UpdateIndividualTaskCommandHandler(IProjectManagementReadRepository readRepository, IProjectManagementWriteRepository writeRepository, IUnitOfWork unitOfWork, IProjectManagementProducer producer)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _producer = producer;
        }

        public async System.Threading.Tasks.Task Handle(UpdateIndividualTaskCommandRequest request, CancellationToken cancellationToken)
        {
            var task = await _readRepository.GetIndividualTask(request.Id, true);
            if (task == null)
            {
                throw new IndividualTaskNotFoundException();
            }

            task.Update(request.TaskTitle, request.Description, request.Deadline);
            await _writeRepository.UpdateIndividualTask(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _producer.PublishAsync("IndividualTaskUpdated", new
            {
                Id = task.Id,
                AssignedUserId = task.AssignedUserId,
                TaskTitle = task.TaskTitle,
                TaskDescription = task.Description,
            });
        }
    }
}