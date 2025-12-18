using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Command.Update
{
    public record UpdateTaskAnswerCommandRequest : IRequest
    {
        public string TaskAnswer {  get; init; }
        public Guid TaskAnswerId { get; init; }
        public Guid TaskId { get; init; }


    }
}
