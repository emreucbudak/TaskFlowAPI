using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Department.Command.AddUserToDepartment
{
    public class AddUserToDepartmentCommandHandler : IRequestHandler<AddUserToDepartmentCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Department, Guid> _departmentReadRepository;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityCapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _identityProducer;
        private readonly ICacheService _cacheService;

        public AddUserToDepartmentCommandHandler(
            IReadRepository<Domain.Entities.Department, Guid> departmentReadRepository,
            UserManager<User> userManager,
            IIdentityCapUnitOfWork unitOfWork,
            ICapPublisher identityProducer,
            ICacheService cacheService)
        {
            _departmentReadRepository = departmentReadRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _identityProducer = identityProducer;
            _cacheService = cacheService;
        }

        public async Task Handle(AddUserToDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_identityProducer, autoCommit: false))
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user is null)
                {
                    throw new UserNotFoundExceptions();
                }

                var targetDepartment = await _departmentReadRepository.GetByIdAsync(
                    true,
                    request.DepartmentId,
                    query => query.Include(item => item.DepartmentMembers));

                if (targetDepartment is null || targetDepartment.CompanyId != user.CompanyId)
                {
                    throw new DepartmentNotFoundExceptions();
                }

                var currentDepartmentIds = await _userManager.Users
                    .Where(item => item.Id == user.Id)
                    .SelectMany(item => item.DepartmentMembers.Select(member => member.DepartmentId))
                    .Distinct()
                    .ToListAsync(cancellationToken);

                foreach (var currentDepartmentId in currentDepartmentIds.Where(item => item != request.DepartmentId))
                {
                    var currentDepartment = await _departmentReadRepository.GetByIdAsync(
                        true,
                        currentDepartmentId,
                        query => query.Include(item => item.DepartmentMembers));

                    if (currentDepartment is null ||
                        !currentDepartment.DepartmentMembers.Any(member => member.UserId == user.Id))
                    {
                        continue;
                    }

                    currentDepartment.RemoveUser(user.Id);

                    await _identityProducer.PublishAsync(
                        "UserRemovedFromDepartment",
                        new UserRemovedFromDepartmentIntegrationEvent(user.Id, currentDepartment.Id));
                }

                var existingMembership = targetDepartment.DepartmentMembers
                    .FirstOrDefault(member => member.UserId == user.Id);

                if (existingMembership is not null)
                {
                    if (existingMembership.DepartmentRoleId != request.RoleId)
                    {
                        targetDepartment.RemoveUser(user.Id);
                        targetDepartment.AddUser(user.Id, request.RoleId);
                    }
                }
                else
                {
                    targetDepartment.AddUser(user.Id, request.RoleId);

                    await _identityProducer.PublishAsync(
                        "UserAddedToDepartment",
                        new UserAddedToDepartmentIntegrationEvent(user.Id, targetDepartment.Id));
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                await _cacheService.RemoveAsync($"getallcompanygroups:{user.CompanyId}");
                await _cacheService.RemoveAsync($"getallcompanyusers:{user.CompanyId}");
            }
        }
    }
}
