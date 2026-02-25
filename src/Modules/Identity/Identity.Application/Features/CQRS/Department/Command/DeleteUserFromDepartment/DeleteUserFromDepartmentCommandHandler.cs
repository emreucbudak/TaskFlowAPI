using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Department.Command.DeleteUserFromDepartment
{
    public class DeleteUserFromDepartmentCommandHandler : IRequestHandler<DeleteUserFromDepartmentCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Department, Guid> _departmentReadRepository;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityCapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _capPublisher;
        private readonly ICacheService _cacheService;

        public DeleteUserFromDepartmentCommandHandler(
            IReadRepository<Domain.Entities.Department, Guid> departmentReadRepository,
            UserManager<User> userManager,
            IIdentityCapUnitOfWork unitOfWork,
            ICapPublisher capPublisher,
            ICacheService cacheService)
        {
            _departmentReadRepository = departmentReadRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _capPublisher = capPublisher;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteUserFromDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false))
            {
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user is null)
                {
                    throw new UserNotFoundExceptions();
                }

                var department = await _departmentReadRepository.GetByIdAsync(true, request.DepartmentId);
                if (department is null)
                {
                    throw new DepartmentNotFoundExceptions();
                }

                department.RemoveUser(user.Id);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _capPublisher.PublishAsync("UserRemovedFromDepartment", new UserRemovedFromDepartmentIntegrationEvent(
                    user.Id,
                    department.Id
                ));

                await transaction.CommitAsync(cancellationToken);
                await _cacheService.RemoveAsync($"getallcompanygroups:{user.CompanyId}");
            }
        }
    }
}
