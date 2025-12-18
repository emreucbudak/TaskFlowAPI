using FlashMediator.src.FlashMediator.Contracts;

namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Command.Create
{
    public record CreateTaskAnswerCommandRequest : IRequest
    {
        public string AnswerText { get; init; }
        public Guid SenderId { get; set; }
        public Guid TaskId { get; set; }
    }
}
