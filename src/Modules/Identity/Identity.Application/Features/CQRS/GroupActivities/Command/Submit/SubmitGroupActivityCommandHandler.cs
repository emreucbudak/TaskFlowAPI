using FlashMediator;
using Identity.Application.Features.CQRS.Groups.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.Services;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.GroupActivities.Command.Submit;

public sealed class SubmitGroupActivityCommandHandler(
    IReadRepository<Domain.Entities.Groups, Guid> groupReadRepository,
    IWriteRepository<GroupActivity> writeRepository,
    IIdentityCapUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<SubmitGroupActivityCommandRequest>
{
    public async Task Handle(SubmitGroupActivityCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId
            ?? throw new AuthExceptions("Kullanici dogrulanamadi.");

        var group = await groupReadRepository.GetByIdAsync(
            trackChanges: false,
            id: request.GroupId,
            inc: q => q.Include(g => g.Users));

        if (group is null)
            throw new GroupsNotFoundExceptions();

        var isMember = group.Users.Any(u => u.UserId == currentUserId);
        if (!isMember)
            throw new AuthExceptions("Bu grubun uyesi degilsiniz.");

        var activity = new GroupActivity(
            request.Title,
            request.Description,
            request.GroupId,
            currentUserId);

        await writeRepository.AddAsync(activity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
