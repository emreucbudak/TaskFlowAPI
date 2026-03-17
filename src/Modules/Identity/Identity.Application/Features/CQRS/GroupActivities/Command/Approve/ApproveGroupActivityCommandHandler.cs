using FlashMediator;
using Identity.Application.Repositories;
using Identity.Application.Services;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.GroupActivities.Command.Approve;

public sealed class ApproveGroupActivityCommandHandler(
    IReadRepository<GroupActivity, Guid> readRepository,
    IIdentityCapUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<ApproveGroupActivityCommandRequest>
{
    private const int GroupLeaderRoleId = 1;

    public async Task Handle(ApproveGroupActivityCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new AuthExceptions("Kullanici dogrulanamadi.");

        var activity = await readRepository.GetByIdAsync(
            trackChanges: true,
            id: request.ActivityId,
            inc: q => q.Include(a => a.Group).ThenInclude(g => g.Users));

        if (activity is null)
            throw new NotFoundExceptions("Aktivite bulunamadi.");

        var isLeader = activity.Group.Users
            .Any(u => u.UserId == currentUserId && u.GroupRolesId == GroupLeaderRoleId);

        if (!isLeader)
            throw new AuthExceptions("Sadece grup liderleri aktiviteleri onaylayabilir.");

        activity.Approve(currentUserId, request.Note);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
