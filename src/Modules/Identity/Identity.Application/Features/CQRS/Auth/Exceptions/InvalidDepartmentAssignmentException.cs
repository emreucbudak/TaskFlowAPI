using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public sealed class InvalidDepartmentAssignmentException : AuthExceptions
    {
        public InvalidDepartmentAssignmentException() : base("Departman atamasi sadece Worker rolunde yapilabilir.")
        {
        }
    }
}
