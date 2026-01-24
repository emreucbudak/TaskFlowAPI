using FlashMediator;
using TaskFlow.BuildingBlocks.UnitOfWork;
using Tenant.Application.Repositories;


namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Update
{
    internal class UpdateCompanyPlanCommandHandler : IRequestHandler<UpdateCompanyPlanCommandRequest>
    {
        private readonly ITenantWriteRepository tenantWriteRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ITenantReadRepository tenantReadRepository;

        public UpdateCompanyPlanCommandHandler(ITenantWriteRepository tenantWriteRepository, IUnitOfWork unitOfWork, ITenantReadRepository tenantReadRepository)
        {
            this.tenantWriteRepository = tenantWriteRepository;
            this.unitOfWork = unitOfWork;
            this.tenantReadRepository = tenantReadRepository;
        }

        public async Task Handle(UpdateCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = await tenantReadRepository.GetPlan(request.CompanyPlanId, true);
            var newProperties = new Domain.Entities.PlanProperties(
                request.PeopleAddedLimit,
                request.TeamLimit,
                request.IsDailyPlannerEnabled,
                request.IsIncludeTaskPriorityCategory,
                request.IsDeadlineNotificationEnabled,
                request.IsIncludeAddTaskNotifications
            );
            companyPlan.UpdateProperties(newProperties);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
