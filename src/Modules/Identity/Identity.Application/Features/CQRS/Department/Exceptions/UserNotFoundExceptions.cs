using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Department.Exceptions
{
    public class UserNotFoundExceptions : NotFoundExceptions
    {
        public UserNotFoundExceptions() : base("Kullanıcı bulunamadı!")
        {
        }
    }
}
