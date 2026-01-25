using FlashMediator;

namespace Identity.Application.Features.CQRS.Auth.Register
{
    public record RegisterCommandRequest : IRequest
    {
        public RegisterCommandRequest(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public string Name { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
