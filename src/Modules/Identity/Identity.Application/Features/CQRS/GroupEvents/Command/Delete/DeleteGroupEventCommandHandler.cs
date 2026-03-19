using FlashMediator;
using Identity.Application.Repositories;
using Identity.Application.Services;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.GroupEvents.Command.Delete;

public sealed class DeleteGroupEventCommandHandler(
    IReadRepository<GroupEvent, Guid> groupEventReadRepository,
    IWriteRepository<GroupEvent> writeRepository,
    IIdentityCapUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteGroupEventCommandRequest>
{
    private const int GroupLeaderRoleId = 1;

    public async Task Handle(DeleteGroupEventCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new AuthExceptions("Kullanici dogrulanamadi.");

        var groupEvent = await groupEventReadRepository.GetByIdAsync(
            trackChanges: false,
            id: request.GroupEventId,
            inc: q => q
                .Include(e => e.Group)
                .ThenInclude(g => g.Users));

        if (groupEvent is null)
        {
            throw new NotFoundExceptions("Etkinlik bulunamadi.");
        }

        var isLeader = groupEvent.Group.Users.Any(u => u.UserId == currentUserId && u.GroupRolesId == GroupLeaderRoleId);
        if (isLeader is false)
        {
            throw new AuthExceptions("Sadece grup lideri etkinlik silebilir.");
        }

        writeRepository.Delete(groupEvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
