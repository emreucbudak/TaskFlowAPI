using FlashMediator;
using Identity.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.Delete
{
    public class DeleteGroupsCommandHandler : IRequestHandler<DeleteGroupsCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups> _readRepository;
        private readonly IWriteRepository<Domain.Entities.Groups> _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteGroupsCommandHandler(IReadRepository<Domain.Entities.Groups> readRepository, IWriteRepository<Domain.Entities.Groups> writeRepository, IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task Handle(DeleteGroupsCommandRequest request, CancellationToken cancellationToken)
        {
            var groups = await _readRepository.GetByIdAsync(request.GroupId);
            if (groups is null)
            {
                throw new KeyNotFoundException("Group not found.");
            }
            await _writeRepository.DeleteAsync(groups);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
