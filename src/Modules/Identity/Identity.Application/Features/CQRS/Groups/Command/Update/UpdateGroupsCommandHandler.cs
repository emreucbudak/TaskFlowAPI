using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.Update
{
    public class UpdateGroupsCommandHandler : IRequestHandler<UpdateGroupsCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups,Guid> _groupReadRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateGroupsCommandHandler(IReadRepository<Domain.Entities.Groups, Guid> groupReadRepository, IUnitOfWork unitOfWork)
        {
            _groupReadRepository = groupReadRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateGroupsCommandRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupReadRepository.GetByIdAsync(true,request.Id);
            if (groups is null)
            {
                throw new GroupsNotFoundExceptions();
            }
            groups.UpdateName(request.Name);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
