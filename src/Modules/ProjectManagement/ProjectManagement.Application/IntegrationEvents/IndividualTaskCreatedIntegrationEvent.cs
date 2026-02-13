namespace ProjectManagement.Application.IntegrationEvents
{
    public record IndividualTaskCreatedIntegrationEvent(
            Guid AssignedUserId,
            string TaskTitle,
            string TaskDescription,
            DateOnly Deadline
        );
}
