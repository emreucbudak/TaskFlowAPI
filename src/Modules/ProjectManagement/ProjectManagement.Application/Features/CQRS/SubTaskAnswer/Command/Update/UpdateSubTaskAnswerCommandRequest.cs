using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Command.Update
{
    public record UpdateSubTaskAnswerCommandRequest : IRequest
    {
        public string SubTaskAnswer {  get; init; }
        public Guid TaskId { get; init; }
        public Guid SubTaskId { get; init; }
        public Guid SubTaskAnswerId { get; init; }

    }
}
