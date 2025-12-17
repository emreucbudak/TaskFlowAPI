using FlashMediator.src.FlashMediator.Contracts;
using Tenant.Application.Repositories;
using Tenant.Application.UnitOfWork;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete
{
    public class DeleteCompanyPlanCommandHandler : IRequestHandler<DeleteCompanyPlanCommandRequest>
    {
        private readonly IUnitOfWork unitOfWork;    
        private readonly ITenantReadRepository tenantReadRepository;
        private readonly ITenantWriteRepository tenantWriteRepository;

        public DeleteCompanyPlanCommandHandler(IUnitOfWork unitOfWork, ITenantReadRepository tenantReadRepository, ITenantWriteRepository tenantWriteRepository)
        {
            this.unitOfWork = unitOfWork;
            this.tenantReadRepository = tenantReadRepository;
            this.tenantWriteRepository = tenantWriteRepository;
        }

        public async Task Handle(DeleteCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = await tenantReadRepository.GetPlan(request.CompanyPlanId,false);
            await tenantWriteRepository.DeletePlan(companyPlan);
            await unitOfWork.SaveChangesAsync();

        }
    }
}
