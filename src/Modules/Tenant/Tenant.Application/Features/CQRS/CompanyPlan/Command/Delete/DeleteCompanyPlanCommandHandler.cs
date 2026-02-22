using FlashMediator;
using Tenant.Application.Repositories;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete
{
    public class DeleteCompanyPlanCommandHandler : IRequestHandler<DeleteCompanyPlanCommandRequest>
    {
        private readonly ITenantReadRepository tenantReadRepository;
        private readonly ITenantWriteRepository tenantWriteRepository;

        public DeleteCompanyPlanCommandHandler(ITenantReadRepository tenantReadRepository, ITenantWriteRepository tenantWriteRepository)
        {
            this.tenantReadRepository = tenantReadRepository;
            this.tenantWriteRepository = tenantWriteRepository;
        }

        public async Task Handle(DeleteCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = await tenantReadRepository.GetPlan(request.CompanyPlanId,false);
            await tenantWriteRepository.DeletePlan(companyPlan);

        }
    }
}
