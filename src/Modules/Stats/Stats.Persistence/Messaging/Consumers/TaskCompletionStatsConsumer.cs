using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Stats.Application.Repositories;
using Stats.Persistence.Messaging.IntegrationEvents;

namespace Stats.Persistence.Messaging.Consumers;

public sealed class TaskCompletionStatsConsumer(
    IWorkerStatsWriteRepositories workerStatsWriteRepository,
    ILogger<TaskCompletionStatsConsumer> logger) : ICapSubscribe
{
    [CapSubscribe("GroupTaskCompleted", Group = "module.stats")]
    public async Task OnGroupTaskCompleted(GroupTaskCompletedIntegrationEvent eventData)
    {
        if (eventData.AssignedUserIds is null || eventData.AssignedUserIds.Count == 0)
        {
            return;
        }

        var userIds = eventData.AssignedUserIds
            .Where(userId => userId != Guid.Empty)
            .Distinct()
            .ToList();

        foreach (var userId in userIds)
        {
            await RecordCompletionSafelyAsync(userId, eventData.CompletedOn, eventData.Deadline, eventData.TaskId);
        }
    }

    [CapSubscribe("IndividualTaskCompleted", Group = "module.stats")]
    public Task OnIndividualTaskCompleted(IndividualTaskCompletedIntegrationEvent eventData)
    {
        if (eventData.AssignedUserId == Guid.Empty)
        {
            return Task.CompletedTask;
        }

        return RecordCompletionSafelyAsync(
            eventData.AssignedUserId,
            eventData.CompletedOn,
            eventData.Deadline,
            eventData.TaskId);
    }

    private async Task RecordCompletionSafelyAsync(
        Guid userId,
        DateOnly completedOn,
        DateOnly deadline,
        Guid taskId)
    {
        try
        {
            await workerStatsWriteRepository.RecordTaskCompletionAsync(userId, completedOn, deadline);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Task completion puani kaydedilemedi. TaskId: {TaskId}, UserId: {UserId}",
                taskId,
                userId);
        }
    }
}
