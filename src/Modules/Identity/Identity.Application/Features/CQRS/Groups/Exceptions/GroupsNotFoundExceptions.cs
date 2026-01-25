using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.Groups.Exceptions
{
    public class GroupsNotFoundExceptions : NotFoundExceptions
    {
        public GroupsNotFoundExceptions() : base($"Grup Bulunamadı!")
        {
        }
    }
}
