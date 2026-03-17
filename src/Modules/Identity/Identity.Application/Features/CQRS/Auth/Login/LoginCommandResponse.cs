namespace Identity.Application.Features.CQRS.Auth.Login
{
    public record LoginCommandResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public Guid UserId { get; init; }
        public Guid CompanyId { get; init; }
        public string Role { get; init; } = string.Empty;
        public bool IsDepartmentLeader { get; init; }
        public Guid? DepartmentId { get; init; }
    }
}
