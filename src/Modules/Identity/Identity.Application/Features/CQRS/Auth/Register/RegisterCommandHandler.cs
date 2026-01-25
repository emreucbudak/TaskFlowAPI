using FlashMediator;
using Identity.Application.Features.CQRS.Auth.Exceptions;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.CQRS.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest>
    {
        private readonly UserManager<User> _userManager;
        public RegisterCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }   
        public async Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            User isRegister = await _userManager.FindByEmailAsync(request.Email);
            if (isRegister is not null)
            {
                throw new AlreadyExistUserExceptions(request.Email);
            }



        }
    }
}
