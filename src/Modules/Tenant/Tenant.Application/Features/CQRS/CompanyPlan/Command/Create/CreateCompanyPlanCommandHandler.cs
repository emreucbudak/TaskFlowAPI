using FlashMediator;
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

        public async Task Handle(CreateCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = new Tenant.Domain.Entities.CompanyPlan(request.PlanName,request.PlanProperties,request.PlanPrice);
            _tenantWriteRepository.AddPlan(companyPlan);
            await _tenantWriteRepository.SaveChangesAsync(cancellationToken);

        }
    }
}
