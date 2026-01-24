using FlashMediator;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Update
{
    public record UpdateCompanyPlanCommandRequest : IRequest
    {
        public int PeopleAddedLimit { get; init; }
        public bool IsIncludeGroupChat { get; init; }
        public int TeamLimit { get; init; }
        public bool IsDailyPlannerEnabled { get; init; }
        public bool IsIncludeTaskPriorityCategory { get; init; }
        public bool IsDeadlineNotificationEnabled { get; init; }
        public bool IsIncludeAddTaskNotifications { get; init; }
        public Guid CompanyPlanId { get; init; }
    }
}
