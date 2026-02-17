using FlashMediator;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Auth.Register
{
    public record RegisterCommandRequest : IRequest , ILimitedQueryable
    {
        public RegisterCommandRequest(string name, string email, string password, Guid companyId, string role)
        {
            Name = name;
            Email = email;
            Password = password;
            CompanyId = companyId;
            Role = role;
        }

        public string Name { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public Guid CompanyId { get; init; }
        public string Role { get; init; }

        public Guid TenantId => CompanyId;

        public LimitType limitType => LimitType.PeopleAdded;
    }
}
