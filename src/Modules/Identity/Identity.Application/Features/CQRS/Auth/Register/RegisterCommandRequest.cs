using FlashMediator;

namespace Identity.Application.Features.CQRS.Auth.Register
{
    public record RegisterCommandRequest : IRequest
    {
        public string Name { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
