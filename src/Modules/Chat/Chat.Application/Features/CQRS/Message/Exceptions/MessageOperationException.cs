using TaskFlow.BuildingBlocks.Bases.Exceptions;

namespace Chat.Application.Features.CQRS.Message.Exceptions
{
    public class MessageOperationException : BaseExceptions
    {
        public MessageOperationException(string message) : base(message)
        {
        }
    }
}
