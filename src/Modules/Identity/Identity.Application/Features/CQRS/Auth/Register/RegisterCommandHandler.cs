using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Features.CQRS.Company.Exceptions;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Auth.Register;

public sealed class RegisterUserCommandHandler(
    UserManager<User> userManager,
    IReadRepository<Domain.Entities.Company, Guid> companyReadRepository,
    IReadRepository<Domain.Entities.Department, Guid> departmentReadRepository,
    RoleManager<Roles> roleManager,
    IIdentityCapUnitOfWork unitOfWork,
    ICapPublisher capPublisher,
    ICacheService cacheService,
    IEnumerable<ISubscriptionLimitCheckerService> limitCheckers) : IRequestHandler<RegisterCommandRequest>
{
    private const int WorkerDepartmentRoleId = 3;

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

        Domain.Entities.Department? department = null;
        if (request.DepartmentId.HasValue)
        {
            if (!string.Equals(request.Role, "Worker", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDepartmentAssignmentException();
            }

            department = await departmentReadRepository.GetByIdAsync(true, request.DepartmentId.Value);
            if (department is null || department.CompanyId != request.CompanyId)
            {
                throw new DepartmentNotFoundExceptions();
            }
        }

        ISubscriptionLimitCheckerService? workerLimitChecker = null;
        var workerSlotReserved = false;

        if (string.Equals(request.Role, "Worker", StringComparison.OrdinalIgnoreCase))
        {
            workerLimitChecker = limitCheckers
                .FirstOrDefault(item => item.LimitType == LimitType.PeopleAdded)
                ?? throw new InvalidOperationException(
                    $"{LimitType.PeopleAdded} limiti icin kontrolcu bulunamadi.");

            await workerLimitChecker.CheckLimitAsync(request.CompanyId);
            workerSlotReserved = true;
        }

        var newUser = User.Create(request.Name, request.Email, request.CompanyId);

        // User creation and role assignment run in one transaction.
        // If any step fails, the transaction is rolled back.
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

            if (department is not null)
            {
                department.AddUser(newUser.Id, WorkerDepartmentRoleId);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            if (workerSlotReserved && workerLimitChecker is not null)
            {
                await workerLimitChecker.ReleaseLimitAsync(request.CompanyId);
            }

            throw;
        }

        await cacheService.RemoveAsync($"getallcompanyusers:{request.CompanyId}");
        await cacheService.RemoveAsync($"getallcompanygroups:{request.CompanyId}");
    }
}
