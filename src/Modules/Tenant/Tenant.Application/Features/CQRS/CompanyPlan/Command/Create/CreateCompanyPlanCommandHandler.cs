using FlashMediator.src.FlashMediator.Contracts;
using Tenant.Application.Repositories;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Create
{
    public class CreateCompanyPlanCommandHandler : IRequestHandler<CreateCompanyPlanCommandRequest>
    {
        private readonly ITenantWriteRepository _tenantWriteRepository;

        public CreateCompanyPlanCommandHandler(ITenantWriteRepository tenantWriteRepository)
        {
            _tenantWriteRepository = tenantWriteRepository;
        }

        public Task Handle(CreateCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
