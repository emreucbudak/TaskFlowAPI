using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Department.Command.AddUserToDepartment
{
    public class AddUserToDepartmentCommandHandler : IRequestHandler<AddUserToDepartmentCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Department, Guid> _departmentReadRepository;
        private readonly UserManager<User> _userManager;
        private readonly ICapUnitOfWork _unitOfWork;
        private readonly ICapPublisher _identityProducer;

        public AddUserToDepartmentCommandHandler(
            IReadRepository<Domain.Entities.Department, Guid> departmentReadRepository,
            UserManager<User> userManager,
            ICapUnitOfWork unitOfWork,
            ICapPublisher identityProducer)
        {
            _departmentReadRepository = departmentReadRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _identityProducer = identityProducer;
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

                var department = await _departmentReadRepository.GetByIdAsync(true, request.DepartmentId);
                if (department is null)
                {
                    throw new DepartmentNotFoundExceptions();
                }

                department.AddUser(user.Id, request.RoleId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _identityProducer.PublishAsync("UserAddedToDepartment", new UserAddedToDepartmentIntegrationEvent(
                    user.Id,
                    department.Id
                ));

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}
