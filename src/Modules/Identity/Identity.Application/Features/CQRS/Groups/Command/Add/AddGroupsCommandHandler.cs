using DotNetCore.CAP;
using FlashMediator;
using FluentValidation;
using FluentValidation.Results;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using Identity.Application.Services;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Exceptions;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Command.Add;

public sealed class CreateGroupCommandHandler(
    IWriteRepository<Domain.Entities.Groups> writeRepository,
    IIdentityCapUnitOfWork unitOfWork,
    ICapPublisher capPublisher,
    ICacheService cacheService,
    ICurrentUserService currentUserService,
    UserManager<User> userManager) : IRequestHandler<AddGroupsCommandRequest>
{
    private const int GroupLeaderRoleId = 1;
    private const int GroupMemberRoleId = 2;
    private const int DepartmentLeaderRoleId = 1;

    public async Task Handle(AddGroupsCommandRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.UserId;
        if (currentUserId is null)
        {
            throw new AuthExceptions("Kullanici dogrulanamadi.");
        }

        var actor = await userManager.FindByIdAsync(currentUserId.Value.ToString());
        if (actor is null)
        {
            throw new AuthExceptions("Kullanici dogrulanamadi.");
        }

        if (actor.CompanyId != request.CompanyId)
        {
            throw new AuthExceptions("Bu sirket icin grup olusturma yetkiniz yok.");
        }

        var actorRoles = await userManager.GetRolesAsync(actor);
        var isCompanyUser = actorRoles.Any(role => string.Equals(role, "Company", StringComparison.OrdinalIgnoreCase));
        var selectedUserIds = request.UserIds
            .Where(userId => userId != Guid.Empty)
            .Distinct()
            .ToList();

        var group = new Domain.Entities.Groups(request.Name, request.CompanyId);

        if (isCompanyUser)
        {
            await AddCompanyMembersAsync(group, actor.CompanyId, selectedUserIds, cancellationToken);
        }
        else
        {
            await AddDepartmentLeaderMembersAsync(group, actor, request, selectedUserIds, cancellationToken);
        }

        await using var transaction = unitOfWork.BeginTransaction(capPublisher, autoCommit: false);
        try
        {
            await writeRepository.AddAsync(group);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var member in group.Users)
            {
                await capPublisher.PublishAsync("UserAddedToGroup", new UserAddedToGroupIntegrationEvent(
                    group.Id,
                    member.UserId,
                    member.GroupRolesId));
            }

            await transaction.CommitAsync(cancellationToken);
            await cacheService.RemoveAsync($"getallcompanygroups:{request.CompanyId}");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task AddCompanyMembersAsync(
        Domain.Entities.Groups group,
        Guid companyId,
        IReadOnlyCollection<Guid> selectedUserIds,
        CancellationToken cancellationToken)
    {
        if (selectedUserIds.Count == 0)
        {
            return;
        }

        var validUserIds = await userManager.Users
            .AsNoTracking()
            .Where(user => user.CompanyId == companyId && selectedUserIds.Contains(user.Id))
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);

        if (validUserIds.Count != selectedUserIds.Count)
        {
            throw CreateValidationException(nameof(AddGroupsCommandRequest.UserIds), "Secilen kullanicilarin tamami sirket icinde bulunamadi.");
        }

        foreach (var userId in validUserIds)
        {
            group.AddUser(userId, GroupMemberRoleId);
        }
    }

    private async Task AddDepartmentLeaderMembersAsync(
        Domain.Entities.Groups group,
        User actor,
        AddGroupsCommandRequest request,
        IReadOnlyCollection<Guid> selectedUserIds,
        CancellationToken cancellationToken)
    {
        var departmentMemberships = await userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == actor.Id)
            .SelectMany(user => user.DepartmentMembers.Select(member => new
            {
                member.DepartmentId,
                DepartmentName = member.Department.Name,
                member.DepartmentRoleId
            }))
            .ToListAsync(cancellationToken);

        var managedDepartmentIds = departmentMemberships
            .Where(member => member.DepartmentRoleId == DepartmentLeaderRoleId)
            .Select(member => member.DepartmentId)
            .Distinct()
            .ToHashSet();

        if (managedDepartmentIds.Count == 0)
        {
            throw new AuthExceptions("Sadece departman liderleri kendi departmanlari icin grup olusturabilir.");
        }

        if (request.DepartmentId is null || request.DepartmentId == Guid.Empty)
        {
            throw CreateValidationException(nameof(AddGroupsCommandRequest.DepartmentId), "Grup olusturmak icin yonettiginiz departmani secin.");
        }

        if (!managedDepartmentIds.Contains(request.DepartmentId.Value))
        {
            throw new AuthExceptions("Yalnizca yonettiginiz departman icin grup olusturabilirsiniz.");
        }

        var selectedDepartmentUserIds = selectedUserIds
            .Where(userId => userId != actor.Id)
            .ToList();

        if (selectedDepartmentUserIds.Count == 0)
        {
            throw CreateValidationException(nameof(AddGroupsCommandRequest.UserIds), "Departmaninizdan en az bir calisan secin.");
        }

        var allowedUserIds = await userManager.Users
            .AsNoTracking()
            .Where(user => user.CompanyId == actor.CompanyId)
            .Where(user => user.DepartmentMembers.Any(member => member.DepartmentId == request.DepartmentId.Value))
            .Select(user => user.Id)
            .ToListAsync(cancellationToken);

        var invalidUserIds = selectedDepartmentUserIds.Except(allowedUserIds).ToList();
        if (invalidUserIds.Count > 0)
        {
            throw CreateValidationException(nameof(AddGroupsCommandRequest.UserIds), "Sadece kendi departmaninizdaki kullanicilari gruba ekleyebilirsiniz.");
        }

        group.AddUser(actor.Id, GroupLeaderRoleId);

        foreach (var userId in selectedDepartmentUserIds)
        {
            group.AddUser(userId, GroupMemberRoleId);
        }
    }

    private static ValidationException CreateValidationException(string propertyName, string message)
    {
        return new ValidationException([
            new ValidationFailure(propertyName, message)
        ]);
    }
}
