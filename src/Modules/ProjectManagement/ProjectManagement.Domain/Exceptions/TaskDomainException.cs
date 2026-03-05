namespace ProjectManagement.Domain.Exceptions
{
    public sealed class TaskDomainException : Exception
    {
        public TaskDomainException(string message) : base(message)
        {
        }

        public TaskDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
