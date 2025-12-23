using FlashMediator.src.FlashMediator.Contracts;

namespace Identity.Application.Features.CQRS.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        public Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
