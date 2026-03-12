using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Auth.Queries.GetAllCompanyUsers
{
    public sealed class GetAllCompanyUsersQueriesRequest : IRequest<List<GetAllCompanyUsersQueriesResponse>>, ICacheableQuery
    {
        public Guid CompanyId { get; init; }

        public string CacheKey => $"getallcompanyusers:{CompanyId}";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(10);
    }
}

