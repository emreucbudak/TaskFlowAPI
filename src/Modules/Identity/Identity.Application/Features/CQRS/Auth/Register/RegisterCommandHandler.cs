using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Features.CQRS.Company.Exceptions;
using Identity.Application.Repositories;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace Identity.Application.Features.CQRS.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest>
    {
        private readonly UserManager<User> _userManager;
        private readonly IReadRepository<Domain.Entities.Company,Guid> _companyReadRepository;
        private readonly RoleManager<Roles> role;
        public RegisterCommandHandler(UserManager<User> userManager, IReadRepository<Domain.Entities.Company, Guid> companyReadRepository, RoleManager<Roles> role)
        {
            _userManager = userManager;
            _companyReadRepository = companyReadRepository;
            this.role = role;
        }
        public async Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            User isRegister = await _userManager.FindByEmailAsync(request.Email);
            if (isRegister is not null)
            {
                throw new AlreadyExistUserExceptions(request.Email);
            }
            var company = await _companyReadRepository.GetByIdAsync(false,request.CompanyId);
            if (company is null )
            {
                throw new CompanyNotFoundExceptions();
            }
            var allowedRoles = new[] { "Company", "Worker" };
            if (!allowedRoles.Contains(request.Role))
            {
                throw new InvalidRoleException();
            }
            var roleExists = await role.RoleExistsAsync(request.Role);
            if (!roleExists)
            {
                throw new RoleNotFoundExceptions();
            }
            User newUser = User.Create(request.Name, request.Email, request.CompanyId);
            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);
            if(!result.Succeeded)
            {
                throw new RegisterNotSuccessfullyExceptions();
            }
            await _userManager.AddToRoleAsync(newUser, request.Role);





        }
    }
}
