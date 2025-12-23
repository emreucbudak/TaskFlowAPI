using FlashMediator.src.FlashMediator.Contracts;

namespace Identity.Application.Features.CQRS.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest>
    {
        public Task Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
