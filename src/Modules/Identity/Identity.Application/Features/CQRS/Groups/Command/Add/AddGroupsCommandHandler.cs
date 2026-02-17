using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.IntegrationEvents;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.Contracts.IntegrationEvents;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.Add;

public sealed class CreateGroupCommandHandler(
    IWriteRepository<Domain.Entities.Groups> writeRepository,
    ICapUnitOfWork unitOfWork,
    ICapPublisher capPublisher) : IRequestHandler<AddGroupsCommandRequest>
{
    public async Task Handle(AddGroupsCommandRequest request, CancellationToken cancellationToken)
    {
        var group = new Domain.Entities.Groups(request.Name, request.companyId);

        // Domain write and CAP outbox publish share the same local transaction.
        // Commit persists both together; any exception rolls both back.
        await using var transaction = unitOfWork.BeginTransaction(capPublisher, autoCommit: false);
        try
        {
            await writeRepository.AddAsync(group);

            await capPublisher.PublishAsync(
                TenantUsageCapTopics.GroupChatCreated,
                new GroupChatCreatedIntegrationEvent(group.Id, group.Name, request.companyId));

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
