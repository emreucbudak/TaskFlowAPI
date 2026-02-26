using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Auth.Command.DeleteWorker;

public sealed class DeleteWorkerCommandHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    ICacheService cacheService,
    IEnumerable<ISubscriptionLimitCheckerService> limitCheckers) : IRequestHandler<DeleteWorkerCommandRequest>
{
    public async Task Handle(DeleteWorkerCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId is null)
        {
            throw new AuthExceptions("Kullanıcı doğrulanamadı.");
        }

        if (request.UserId == Guid.Empty)
        {
            throw new UserNotFoundExceptions();
        }

        if (request.UserId == currentUserId.Value)
        {
            throw new AuthExceptions("Kendi hesabınızı bu ekrandan silemezsiniz.");
        }

        var actor = await userManager.FindByIdAsync(currentUserId.Value.ToString());
        if (actor is null)
        {
            throw new AuthExceptions("Kullanıcı doğrulanamadı.");
        }

        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            throw new UserNotFoundExceptions();
        }

        if (user.CompanyId != actor.CompanyId)
        {
            throw new AuthExceptions("Bu çalışanı silme yetkiniz yok.");
        }

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Any(role => string.Equals(role, "Worker", StringComparison.OrdinalIgnoreCase)))
        {
            throw new AuthExceptions("Sadece çalışan rolündeki kullanıcılar silinebilir.");
        }

        var deleteResult = await userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            throw new DeleteWorkerNotSuccessfullyExceptions();
        }

        var workerLimitChecker = limitCheckers
            .FirstOrDefault(item => item.LimitType == LimitType.PeopleAdded);
        if (workerLimitChecker is not null)
        {
            await workerLimitChecker.ReleaseLimitAsync(user.CompanyId);
        }

        await cacheService.RemoveAsync($"getallcompanyusers:{user.CompanyId}");
        await cacheService.RemoveAsync($"getallcompanygroups:{user.CompanyId}");
    }
}
