using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.AddGroupsMember
{
    public class AddGroupsMemberCommandHandler : IRequestHandler<AddGroupsMemberCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _groupsReadRepository;
        private readonly IIdentityCapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;

        public AddGroupsMemberCommandHandler(
            IReadRepository<Domain.Entities.Groups, Guid> groupsReadRepository,
            IIdentityCapUnitOfWork unitOfWork,
            ICapPublisher capPublisher)
        {
            _groupsReadRepository = groupsReadRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
        }

        public async Task Handle(AddGroupsMemberCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var groups = await _groupsReadRepository.GetByIdAsync(true, request.GroupId);

                if (groups is null)
                {
                    throw new GroupsNotFoundExceptions();
                }

                groups.AddUser(request.UserId, request.RolesId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("UserAddedToGroup", new UserAddedToGroupIntegrationEvent(
                    request.GroupId,
                    request.UserId,
                    request.RolesId
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
