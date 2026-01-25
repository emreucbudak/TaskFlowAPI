using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Application.Features.CQRS.Company.Exceptions;
using Identity.Application.Repositories;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.CQRS.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest>
    {
        private readonly UserManager<User> _userManager;
        private readonly IReadRepository<Domain.Entities.Company,Guid> _companyReadRepository;
        public RegisterCommandHandler(UserManager<User> userManager, IReadRepository<Domain.Entities.Company, Guid> companyReadRepository)
        {
            _userManager = userManager;
            _companyReadRepository = companyReadRepository;
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
            User newUser = User.Create(request.Name, request.Email, request.CompanyId);
            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);
            if(!result.Succeeded)
            {
                throw new RegisterNotSuccessfullyExceptions();
            }
            




        }
    }
}
