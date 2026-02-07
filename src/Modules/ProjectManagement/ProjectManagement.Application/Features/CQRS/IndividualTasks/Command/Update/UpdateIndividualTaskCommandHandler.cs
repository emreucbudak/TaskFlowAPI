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

        public UpdateIndividualTaskCommandHandler(IProjectManagementReadRepository readRepository, IProjectManagementWriteRepository writeRepository, IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
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
        }
    }
}
