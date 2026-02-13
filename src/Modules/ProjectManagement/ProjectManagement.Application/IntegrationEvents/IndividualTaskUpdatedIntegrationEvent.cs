

namespace ProjectManagement.Application.IntegrationEvents
{
    public record IndividualTaskUpdatedIntegrationEvent(Guid Id, Guid AssignedUserId, string TaskTitle, string TaskDescription);
}
