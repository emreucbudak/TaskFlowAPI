using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public class RegisterNotSuccessfullyExceptions : BadRequestExceptions
    {
        public RegisterNotSuccessfullyExceptions() : base("Kayıt işlemi başarısız oldu!")
        {
        }
    }
}
