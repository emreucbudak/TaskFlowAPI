using TaskFlow.BuildingBlocks.Bases.Exceptions;
using TaskFlow.BuildingBlocks.Exceptions;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions
{
    public class IndividualTaskNotFoundException : NotFoundExceptions
    {
        public IndividualTaskNotFoundException() : base($"Bireysel Görev Bulunamadý!")
        {
        }
    }
}
