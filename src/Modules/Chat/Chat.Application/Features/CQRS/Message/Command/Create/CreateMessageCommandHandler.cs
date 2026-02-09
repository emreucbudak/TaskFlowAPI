using Chat.Application.Services;
using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.Create
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommandRequest>
    {
        private readonly IMessageControlService _messageControlService;

        public CreateMessageCommandHandler(IMessageControlService messageControlService)
        {
            _messageControlService = messageControlService;
        }

        public async Task Handle(CreateMessageCommandRequest request, CancellationToken cancellationToken)
        {
            await _messageControlService.HandleMessageCreationAsync(request, cancellationToken);
        }
    }
}
