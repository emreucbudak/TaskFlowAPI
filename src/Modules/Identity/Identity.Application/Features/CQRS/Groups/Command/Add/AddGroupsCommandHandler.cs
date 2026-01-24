using FlashMediator;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.Add
{
    public class AddGroupsCommandHandler : IRequestHandler<AddGroupsCommandRequest>
    {
        private readonly IWriteRepository<Identity.Domain.Entities.Groups> _writeRepository;
        private readonly IUnitOfWork unitOfWork;

        public AddGroupsCommandHandler(IWriteRepository<Domain.Entities.Groups> writeRepository, IUnitOfWork unitOfWork)
        {
            _writeRepository = writeRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(AddGroupsCommandRequest request, CancellationToken cancellationToken)
        {
            var group = new Domain.Entities.Groups(request.Name);
           await _writeRepository.AddAsync(group);
            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
