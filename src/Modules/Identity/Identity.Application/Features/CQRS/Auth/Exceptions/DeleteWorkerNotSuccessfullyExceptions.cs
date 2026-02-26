using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions;

public sealed class DeleteWorkerNotSuccessfullyExceptions : AuthExceptions
{
    public DeleteWorkerNotSuccessfullyExceptions() : base("Çalışan silme işlemi başarısız oldu.")
    {
    }
}
