using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Auth.Exceptions
{
    public class UserNotFoundExceptions : NotFoundExceptions
    {
        public UserNotFoundExceptions() : base("Kullanıcı bulunamadı!")
        {
        }

        public UserNotFoundExceptions(string email) : base($"{email} emailine  sahip kullanıcı bulunamadı!")
        {
        }
    }
}
