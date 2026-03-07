using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Command.DeleteGroupsMember
{
    public class DeleteGroupsMemberCommandHandler : IRequestHandler<DeleteGroupsMemberCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _groupsReadRepository;
        private readonly IIdentityCapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;
        private readonly ICacheService _cacheService;

        public DeleteGroupsMemberCommandHandler(
            IReadRepository<Domain.Entities.Groups, Guid> groupsReadRepository,
            IIdentityCapUnitOfWork unitOfWork,
            ICapPublisher capPublisher,
            ICacheService cacheService)
        {
            _groupsReadRepository = groupsReadRepository;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteGroupsMemberCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var groups = await _groupsReadRepository.GetByIdAsync(true, request.GroupId);

                if (groups is null)
                {
                    throw new GroupsNotFoundExceptions();
                }

                groups.RemoveUser(request.UserId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("UserRemovedFromGroup", new UserRemovedFromGroupIntegrationEvent(
                    request.GroupId,
                    request.UserId
                ));

                await transaction.CommitAsync(cancellationToken);
                await _cacheService.RemoveAsync($"getallcompanygroups:{groups.CompanyId}");
            }
        }
    }
}
