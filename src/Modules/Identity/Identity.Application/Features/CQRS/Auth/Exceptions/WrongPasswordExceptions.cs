using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public class WrongPasswordExceptions : AuthExceptions
    {
        public WrongPasswordExceptions() : base($"Şifre yanlış!")
        {
        }
    }
}
