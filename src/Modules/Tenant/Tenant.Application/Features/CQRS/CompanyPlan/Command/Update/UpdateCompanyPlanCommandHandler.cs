using FlashMediator;
using Tenant.Application.Repositories;


namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Update
{
    internal class UpdateCompanyPlanCommandHandler : IRequestHandler<UpdateCompanyPlanCommandRequest>
    {
        private readonly ITenantWriteRepository tenantWriteRepository;
        private readonly ITenantReadRepository tenantReadRepository;

        public UpdateCompanyPlanCommandHandler(ITenantWriteRepository tenantWriteRepository, ITenantReadRepository tenantReadRepository)
        {
            this.tenantWriteRepository = tenantWriteRepository;
            this.tenantReadRepository = tenantReadRepository;
        }

        public async Task Handle(UpdateCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = await tenantReadRepository.GetPlan(request.CompanyPlanId, true);
            var newProperties = new Domain.Entities.PlanProperties(
                request.PeopleAddedLimit,
                request.TeamLimit,
                request.IndividualTaskLimit,
                request.IsIncludeReporting
            );
            companyPlan.UpdateProperties(newProperties);
            await tenantWriteRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
