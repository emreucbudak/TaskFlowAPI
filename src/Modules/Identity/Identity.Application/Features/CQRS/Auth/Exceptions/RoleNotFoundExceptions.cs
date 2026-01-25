using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public class RoleNotFoundExceptions : NotFoundExceptions
    {
        public RoleNotFoundExceptions() : base("Rol Bulunamadı!")
        {
        }
    }
}
