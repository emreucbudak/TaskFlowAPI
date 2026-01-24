using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;
using Tenant.Application.Repositories;


namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Create
{
    public class CreateCompanyPlanCommandHandler : IRequestHandler<CreateCompanyPlanCommandRequest>
    {
        private readonly ITenantWriteRepository _tenantWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompanyPlanCommandHandler(ITenantWriteRepository tenantWriteRepository, IUnitOfWork unitOfWork)
        {
            _tenantWriteRepository = tenantWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = new Tenant.Domain.Entities.CompanyPlan(request.PlanName,request.PlanProperties);
            await _tenantWriteRepository.AddPlan(companyPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
