using TaskFlow.BuildingBlocks.Exceptions;

namespace Chat.Application.Features.CQRS.Message.Exceptions
{
    public class MessageNotFoundException : NotFoundExceptions
    {
        public MessageNotFoundException(Guid messageId) 
            : base($"Message with ID {messageId} was not found.")
        {
        }
    }
}
