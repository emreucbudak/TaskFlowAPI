using TaskFlow.BuildingBlocks.Exceptions;

namespace Chat.Application.Features.CQRS.Message.Exceptions
{
    public class MessageOwnershipException : AuthExceptions
    {
        public MessageOwnershipException(Guid messageId)
            : base($"You do not have permission to modify message with ID {messageId}.")
        {
        }
    }
}
