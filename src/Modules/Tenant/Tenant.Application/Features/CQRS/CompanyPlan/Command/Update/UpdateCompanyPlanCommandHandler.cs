using FlashMediator.src.FlashMediator.Contracts;
using Tenant.Application.Repositories;
using Tenant.Application.UnitOfWork;

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
                request.IsIncludeGroupChat,
                request.TeamLimit,
                request.IsDailyPlannerEnabled,
                request.IsIncludeTaskPriorityCategory,
                request.IsDeadlineNotificationEnabled,
                request.IsIncludeAddTaskNotifications
            );
            companyPlan.UpdateProperties(newProperties);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
