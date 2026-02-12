using TaskFlow.BuildingBlocks.Exceptions;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Exceptions
{
    public class SubTaskAnswerNotFoundExceptions : NotFoundExceptions
    {
        public SubTaskAnswerNotFoundExceptions() : base("Görev cevabı bulunamadı!")
        {
        }
    }
}
