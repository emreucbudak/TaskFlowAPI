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

        public DeleteIndividualTaskCommandHandler(IProjectManagementReadRepository readRepository, IProjectManagementWriteRepository writeRepository, IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
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
        }
    }
}
