using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Command.Update
{
    public class UpdateGroupsCommandHandler : IRequestHandler<UpdateGroupsCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _groupReadRepository;
        private readonly IIdentityCapUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public UpdateGroupsCommandHandler(
            IReadRepository<Domain.Entities.Groups, Guid> groupReadRepository,
            IIdentityCapUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _groupReadRepository = groupReadRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(UpdateGroupsCommandRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupReadRepository.GetByIdAsync(true, request.Id);
            if (groups is null)
            {
                throw new GroupsNotFoundExceptions();
            }

            groups.UpdateName(request.Name);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync($"getallcompanygroups:{groups.CompanyId}");
        }
    }
}
