using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Command.Add;

public sealed class CreateGroupCommandHandler(
    IWriteRepository<Domain.Entities.Groups> writeRepository,
    IIdentityCapUnitOfWork unitOfWork,
    ICapPublisher capPublisher,
    ICacheService cacheService) : IRequestHandler<AddGroupsCommandRequest>
{
    public async Task Handle(AddGroupsCommandRequest request, CancellationToken cancellationToken)
    {
        var group = new Domain.Entities.Groups(request.Name, request.companyId);

        await using var transaction = unitOfWork.BeginTransaction(capPublisher, autoCommit: false);
        try
        {
            await writeRepository.AddAsync(group);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await cacheService.RemoveAsync($"getallcompanygroups:{request.companyId}");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
