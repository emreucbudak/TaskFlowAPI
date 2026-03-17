using TaskFlow.BuildingBlocks.Bases.Exceptions;

namespace TaskFlow.BuildingBlocks.Exceptions
{
    public class ConflictExceptions : BaseExceptions
    {
        public ConflictExceptions(string message) : base(message)
        {
        }
    }
}
