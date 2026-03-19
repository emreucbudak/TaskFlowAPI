using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.Services;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.GroupEvents.Command.Create;

public sealed class CreateGroupEventCommandHandler(
    IReadRepository<Domain.Entities.Groups, Guid> groupReadRepository,
    IWriteRepository<GroupEvent> writeRepository,
    IIdentityCapUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<CreateGroupEventCommandRequest, Guid>
{
    private const int GroupLeaderRoleId = 1;

    public async Task<Guid> Handle(CreateGroupEventCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new AuthExceptions("Kullanici dogrulanamadi.");

        var group = await groupReadRepository.GetByIdAsync(
            trackChanges: false,
            id: request.GroupId,
            inc: q => q.Include(g => g.Users));

        if (group is null)
        {
            throw new GroupsNotFoundExceptions();
        }

        var isLeader = group.Users.Any(u => u.UserId == currentUserId && u.GroupRolesId == GroupLeaderRoleId);
        if (!isLeader)
        {
            throw new AuthExceptions("Sadece grup lideri etkinlik olusturabilir.");
        }

        var groupEvent = new GroupEvent(
            request.Subject,
            request.EventType,
            request.Title,
            request.Description,
            request.StartsAt,
            request.EndsAt,
            request.MeetingLink,
            request.GroupId,
            currentUserId);

        await writeRepository.AddAsync(groupEvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return groupEvent.Id;
    }
}
