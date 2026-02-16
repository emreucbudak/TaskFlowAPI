namespace TaskFlow.BuildingBlocks.Exceptions
{
    public class SubscriptionLimitExceededException : Exception
    {
        public SubscriptionLimitExceededException(string message) : base(message)
        {
        }
    }
}