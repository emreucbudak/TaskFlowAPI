using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public class RegisterNotSuccessfullyExceptions : AuthExceptions
    {
        public RegisterNotSuccessfullyExceptions() : base($"Kayıt işlemi başarısız oldu!")
        {
        }
    }
}
