using TaskFlow.BuildingBlocks.Bases.Exceptions;

namespace TaskFlow.BuildingBlocks.Exceptions
{
    public class BadRequestExceptions : BaseExceptions
    {
        public BadRequestExceptions(string message) : base(message)
        {
        }
    }
}
