using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Messaging;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.AddGroupsMember
{
    public class AddGroupsMemberCommandHandler : IRequestHandler<AddGroupsMemberCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _groupsReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityProducer _identityProducer;

        public AddGroupsMemberCommandHandler(IReadRepository<Domain.Entities.Groups, Guid> groupsReadRepository, IUnitOfWork unitOfWork, IIdentityProducer identityProducer)
        {
            _groupsReadRepository = groupsReadRepository;
            _unitOfWork = unitOfWork;
            _identityProducer = identityProducer;
        }

        public async Task Handle(AddGroupsMemberCommandRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupsReadRepository.GetByIdAsync(true,request.GroupId);
            if (groups is null)
            {
                throw new GroupsNotFoundExceptions();
            }
            groups.AddUser(request.UserId, request.RolesId);
            await _identityProducer.PublishAsync("UserAddedToGroup", new
            {
                GroupId = request.GroupId,
                UserId = request.UserId,
                RolesId = request.RolesId
            });
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
