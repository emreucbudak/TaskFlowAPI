using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Messaging;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.DeleteGroupsMember
{
    public class DeleteGroupsMemberCommandHandler : IRequestHandler<DeleteGroupsMemberCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _groupsReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityProducer _identityProducer;

        public DeleteGroupsMemberCommandHandler(IReadRepository<Domain.Entities.Groups, Guid> groupsReadRepository, IUnitOfWork unitOfWork, IIdentityProducer identityProducer)
        {
            _groupsReadRepository = groupsReadRepository;
            _unitOfWork = unitOfWork;
            _identityProducer = identityProducer;
        }

        public async Task Handle(DeleteGroupsMemberCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var groups = await _groupsReadRepository.GetByIdAsync(true, request.GroupId);
                if (groups is null)
                {
                    throw new GroupsNotFoundExceptions();
                }

                groups.RemoveUser(request.UserId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _identityProducer.PublishAsync("UserRemovedFromGroup", new
                {
                    GroupId = request.GroupId,
                    UserId = request.UserId
                });
            }
            catch (GroupsNotFoundExceptions)
            {
                throw new GroupsNotFoundExceptions();
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Grup üyesi silinirken beklenmedik bir hata oluştu: {ex.Message}");
            }
        }
    }
}