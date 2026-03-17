using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public class InvalidRoleException : BadRequestExceptions
    {
        public InvalidRoleException() : base("Bilinmeyen bir rol seçimi!")
        {
        }
    }
}
