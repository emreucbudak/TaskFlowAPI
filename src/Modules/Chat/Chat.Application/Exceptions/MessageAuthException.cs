using TaskFlow.BuildingBlocks.Bases.Exceptions;

namespace Chat.Application.Exceptions
{
    public class MessageAuthException : BaseExceptions
    {
        public MessageAuthException(string message) : base(message)
        {
        }
    }
}