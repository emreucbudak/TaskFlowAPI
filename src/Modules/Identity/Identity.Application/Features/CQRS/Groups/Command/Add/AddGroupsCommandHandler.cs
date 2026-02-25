using DotNetCore.CAP;
using FlashMediator;
using Identity.Application.Repositories;
using Identity.Application.UnitOfWork;

namespace Identity.Application.Features.CQRS.Groups.Command.Add;

public sealed class CreateGroupCommandHandler(
    IWriteRepository<Domain.Entities.Groups> writeRepository,
    IIdentityCapUnitOfWork unitOfWork,
    ICapPublisher capPublisher) : IRequestHandler<AddGroupsCommandRequest>
{
    public async Task Handle(AddGroupsCommandRequest request, CancellationToken cancellationToken)
    {
        var group = new Domain.Entities.Groups(request.Name, request.companyId);

        // Group write is transactional; any exception rolls it back.
        await using var transaction = unitOfWork.BeginTransaction(capPublisher, autoCommit: false);
        try
        {
            await writeRepository.AddAsync(group);

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
