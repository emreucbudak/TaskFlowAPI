using DotNetCore.CAP;
using FlashMediator;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.Contracts.IntegrationEvents;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create;

public sealed class CreateIndividualTaskCommandHandler(
    IProjectManagementWriteRepository writeRepository,
    ICapUnitOfWork unitOfWork,
    ICapPublisher capPublisher) : IRequestHandler<CreateIndividualTaskCommandRequest>
{
    public async Task Handle(CreateIndividualTaskCommandRequest request, CancellationToken cancellationToken)
    {
        var task = new Domain.Entities.IndividualTasks(
            request.AssignedUserId,
            request.TaskTitle,
            request.Description,
            request.Deadline);

        // CAP outbox row and IndividualTask row are part of one EF transaction.
        // If publish/save/commit fails, the transaction is rolled back and neither side is persisted.
        await using var transaction = unitOfWork.BeginTransaction(capPublisher, autoCommit: false);
        try
        {
            await writeRepository.AddIndividualTask(task);

            await capPublisher.PublishAsync(
                TenantUsageCapTopics.IndividualTaskCreated,
                new IndividualTaskCreatedIntegrationEvent(
                    task.Id,
                    task.AssignedUserId,
                    task.TaskTitle,
                    task.Description,
                    task.Deadline,
                    request.CompanyId));

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
