using TaskFlow.BuildingBlocks.Bases.Exceptions;

namespace Chat.Application.Exceptions
{
    public class MessageControlException : BaseExceptions
    {
        public MessageControlException(string message) : base(message)
        {
        }
    }
}