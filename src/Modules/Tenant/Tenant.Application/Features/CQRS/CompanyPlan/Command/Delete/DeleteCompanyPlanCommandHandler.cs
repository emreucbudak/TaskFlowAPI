using FlashMediator.src.FlashMediator.Contracts;
using Tenant.Application.Repositories;
using Tenant.Application.UnitOfWork;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete
{
    public class DeleteCompanyPlanCommandHandler : IRequestHandler<DeleteCompanyPlanCommandRequest>
    {
        private readonly IUnitOfWork unitOfWork;    
        private readonly ITenantWriteRepository tenantWriteRepository;

        public DeleteCompanyPlanCommandHandler(IUnitOfWork unitOfWork, ITenantWriteRepository tenantWriteRepository)
        {
            this.unitOfWork = unitOfWork;
            this.tenantWriteRepository = tenantWriteRepository;
        }

        public async Task Handle(DeleteCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = await tenantWriteRepository.GetPlan(request.CompanyPlanId,false);
            await tenantWriteRepository.DeletePlan(companyPlan);
            await unitOfWork.SaveChangesAsync();

        }
    }
}
