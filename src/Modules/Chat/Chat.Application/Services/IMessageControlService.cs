using Chat.Application.Features.CQRS.Message.Command.Create;

namespace Chat.Application.Services
{
    public interface IMessageControlService
    {
        Task HandleMessageCreationAsync(CreateMessageCommandRequest request, CancellationToken cancellationToken);
    }
}
