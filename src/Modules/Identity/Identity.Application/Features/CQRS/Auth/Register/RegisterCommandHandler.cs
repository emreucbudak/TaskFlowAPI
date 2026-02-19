using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Features.CQRS.Company.Exceptions;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.Contracts.IntegrationEvents;

namespace Identity.Application.Features.CQRS.Auth.Register;

public sealed class RegisterUserCommandHandler(
    UserManager<User> userManager,
    IReadRepository<Domain.Entities.Company, Guid> companyReadRepository,
    RoleManager<Roles> roleManager,
    IIdentityCapUnitOfWork unitOfWork,
    ICapPublisher capPublisher) : IRequestHandler<RegisterCommandRequest>
{
    public async Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new AlreadyExistUserExceptions(request.Email);
        }

        var company = await companyReadRepository.GetByIdAsync(false, request.CompanyId);
        if (company is null)
        {
            throw new CompanyNotFoundExceptions();
        }

        var allowedRoles = new[] { "Company", "Worker" };
        if (!allowedRoles.Contains(request.Role))
        {
            throw new InvalidRoleException();
        }

        var roleExists = await roleManager.RoleExistsAsync(request.Role);
        if (!roleExists)
        {
            throw new RoleNotFoundExceptions();
        }

        var newUser = User.Create(request.Name, request.Email, request.CompanyId);

        // CAP writes its outbox row in the same EF transaction below.
        // If create-role-publish fails at any point, commit never happens and both DB + message are rolled back.
        await using var transaction = unitOfWork.BeginTransaction(capPublisher, autoCommit: false);
        try
        {
            var createResult = await userManager.CreateAsync(newUser, request.Password);
            if (!createResult.Succeeded)
            {
                throw new RegisterNotSuccessfullyExceptions();
            }

            var addRoleResult = await userManager.AddToRoleAsync(newUser, request.Role);
            if (!addRoleResult.Succeeded)
            {
                throw new RegisterNotSuccessfullyExceptions();
            }

            await capPublisher.PublishAsync(
                TenantUsageCapTopics.UserRegistered,
                new UserRegisteredIntegrationEvent(newUser.Id, newUser.Email!, newUser.Name, request.CompanyId));

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
