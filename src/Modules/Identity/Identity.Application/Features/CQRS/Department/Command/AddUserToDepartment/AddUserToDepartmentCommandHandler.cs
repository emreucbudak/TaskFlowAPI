using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Features.CQRS.Department.Exceptions;
using Identity.Application.Messaging;
using Identity.Application.Repositories;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Department.Command.AddUserToDepartment
{
    public class AddUserToDepartmentCommandHandler : IRequestHandler<AddUserToDepartmentCommandRequest>
    {
        private readonly IReadRepository<Domain.Entities.Department, Guid> _departmentReadRepository;
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityProducer identityProducer;

        public AddUserToDepartmentCommandHandler(IReadRepository<Domain.Entities.Department, Guid> departmentReadRepository, UserManager<User> userManager, IUnitOfWork unitOfWork, IIdentityProducer identityProducer)
        {
            _departmentReadRepository = departmentReadRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            this.identityProducer = identityProducer;
        }

        public async Task Handle(AddUserToDepartmentCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                throw new Department.Exceptions.UserNotFoundExceptions();
            }
            var department = await _departmentReadRepository.GetByIdAsync(true,request.DepartmentId);
            if (department is null)
            {
                throw new DepartmentNotFoundExceptions();
            }
            department.AddUser(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await identityProducer.PublishAsync("UserAddedToDepartment",new
            {
                UserId = user.Id,
                DepartmentId = department.Id
            });
        }
    }
}
