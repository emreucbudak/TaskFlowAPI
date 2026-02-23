using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Department.Query.GetAll
{
    public sealed class GetAllCompanyDepartmentsQueriesRequest : IRequest<List<GetAllCompanyDepartmentsQueriesResponse>>, ICacheableQuery
    {
        public Guid CompanyId { get; init; }

        public string CacheKey => $"getallcompanydepartments:{CompanyId}";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(10);
    }
}
