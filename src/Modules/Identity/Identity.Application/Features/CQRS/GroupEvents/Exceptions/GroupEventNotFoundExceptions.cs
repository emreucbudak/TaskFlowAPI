using TaskFlow.BuildingBlocks.Exceptions;

namespace Identity.Application.Features.CQRS.GroupEvents.Exceptions
{
    public class GroupEventNotFoundExceptions : NotFoundExceptions
    {
        public GroupEventNotFoundExceptions() : base("Grup etkinligi bulunamadi!")
        {
        }
    }
}
