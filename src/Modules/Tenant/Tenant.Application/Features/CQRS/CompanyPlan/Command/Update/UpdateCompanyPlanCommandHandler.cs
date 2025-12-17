using FlashMediator.src.FlashMediator.Contracts;
using Tenant.Application.Repositories;
using Tenant.Application.UnitOfWork;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Update
{
    internal class UpdateCompanyPlanCommandHandler : IRequestHandler<UpdateCompanyPlanCommandRequest>
    {
        private readonly ITenantWriteRepository tenantWriteRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateCompanyPlanCommandHandler(ITenantWriteRepository tenantWriteRepository, IUnitOfWork unitOfWork)
        {
            this.tenantWriteRepository = tenantWriteRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCompanyPlanCommandRequest request, CancellationToken cancellationToken)
        {
            var companyPlan = await tenantWriteRepository.GetPlan(request.CompanyPlanId, true);
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
