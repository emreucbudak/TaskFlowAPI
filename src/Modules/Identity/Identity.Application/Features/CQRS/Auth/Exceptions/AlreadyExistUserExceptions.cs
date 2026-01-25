using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public class AlreadyExistUserExceptions : AuthExceptions
    {
        public AlreadyExistUserExceptions(string email) : base($"{email} emaili zaten kullanılıyor!")
        {
        }
    
    }
}
