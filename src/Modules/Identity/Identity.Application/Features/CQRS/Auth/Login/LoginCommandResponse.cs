namespace Identity.Application.Features.CQRS.Auth.Login
{
    public record LoginCommandResponse
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
