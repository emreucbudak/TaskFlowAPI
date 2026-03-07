using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Command.Delete
{
    public class DeleteGroupsCommandHandler : IRequestHandler<DeleteGroupsCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _readRepository;
        private readonly IWriteRepository<Domain.Entities.Groups> _writeRepository;
        private readonly IIdentityCapUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public DeleteGroupsCommandHandler(
            IReadRepository<Domain.Entities.Groups, Guid> readRepository,
            IWriteRepository<Domain.Entities.Groups> writeRepository,
            IIdentityCapUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteGroupsCommandRequest request, CancellationToken cancellationToken)
        {
            var groups = await _readRepository.GetByIdAsync(false, request.GroupId);
            if (groups is null)
            {
                throw new GroupsNotFoundExceptions();
            }

            _writeRepository.Delete(groups);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.RemoveAsync($"getallcompanygroups:{groups.CompanyId}");
        }
    }
}
