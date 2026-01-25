using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Repositories;

namespace Identity.Application.Features.CQRS.Groups.Command.Update
{
    public class UpdateGroupsCommandHandler : IRequestHandler<UpdateGroupsCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups,Guid> _groupReadRepository;
        private readonly IWriteRepository<Domain.Entities.Groups> _groupWriteRepository;

        public UpdateGroupsCommandHandler(IReadRepository<Domain.Entities.Groups, Guid> groupReadRepository, IWriteRepository<Domain.Entities.Groups> groupWriteRepository)
        {
            _groupReadRepository = groupReadRepository;
            _groupWriteRepository = groupWriteRepository;
        }

        public async Task Handle(UpdateGroupsCommandRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupReadRepository.GetByIdAsync(false,request.Id);
            if (groups is null)
            {
                throw new GroupsNotFoundExceptions();
            }
            groups.UpdateName(request.Name);
        }
    }
}
