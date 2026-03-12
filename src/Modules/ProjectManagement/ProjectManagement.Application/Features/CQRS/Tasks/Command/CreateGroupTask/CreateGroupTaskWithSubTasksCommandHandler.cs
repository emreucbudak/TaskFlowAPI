using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Application.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.CreateGroupTask;

public class CreateGroupTaskWithSubTasksCommandHandler : IRequestHandler<CreateGroupTaskWithSubTasksCommandRequest, Guid>
{
    private readonly IProjectManagementWriteRepository _writeRepository;
    private readonly IProjectManagementCapUnitOfWork _unitOfWork;
    private readonly ICapPublisher _capPublisher;

    public CreateGroupTaskWithSubTasksCommandHandler(
        IProjectManagementCapUnitOfWork unitOfWork,
        IProjectManagementWriteRepository writeRepository,
        ICapPublisher capPublisher)
    {
        _unitOfWork = unitOfWork;
        _writeRepository = writeRepository;
        _capPublisher = capPublisher;
    }

    public async Task<Guid> Handle(CreateGroupTaskWithSubTasksCommandRequest request, CancellationToken cancellationToken)
    {
        var deadlineDate = DateOnly.FromDateTime(request.DeadlineTime);
        if (deadlineDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ArgumentException("Bitis tarihi suandan once olamaz.");
        }

        if (request.TaskPriorityCategoryId <= 0)
        {
            throw new ArgumentException("Gorev onceligi zorunludur.");
        }

        if (request.SubTaskAssignments is null || request.SubTaskAssignments.Count == 0)
        {
            throw new ArgumentException("En az bir alt gorev atamasi gereklidir.");
        }

        var task = new Domain.Entities.Task(
            request.TaskName,
            request.Description,
            deadlineDate,
            DateOnly.FromDateTime(DateTime.UtcNow));
        task.UpdateTaskPriority(request.TaskPriorityCategoryId);

        var delayTime = deadlineDate
            .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            .AddDays(-1) - DateTime.UtcNow;

        if (delayTime < TimeSpan.Zero)
        {
            delayTime = TimeSpan.Zero;
        }

        await using var transaction = _unitOfWork.BeginTransaction(_capPublisher, autoCommit: false);
        try
        {
            await _writeRepository.AddTask(task);

            foreach (var assignment in request.SubTaskAssignments)
            {
                var subTask = task.AddSubTask(
                    description: assignment.Description,
                    AssignedUserId: assignment.AssignedUserId,
                    Title: assignment.TaskTitle,
                    taskId: task.Id);

                await _writeRepository.AddSubTask(subTask, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _capPublisher.PublishDelayAsync(
                delayTime,
                "TaskCreated",
                new TaskCreatedIntegrationEvent(
                    task.Id,
                    task.TaskName,
                    task.Description,
                    task.DeadlineTime),
                cancellationToken: cancellationToken);

            foreach (var assignment in request.SubTaskAssignments)
            {
                await _capPublisher.PublishAsync(
                    "SubTaskCreated",
                    new SubTaskCreatedIntegrationEvent(
                        task.Id,
                        assignment.TaskTitle,
                        assignment.Description,
                        assignment.AssignedUserId),
                    cancellationToken: cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return task.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
