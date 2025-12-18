namespace ProjectManagement.Application.Features.CQRS.TaskAnswer.Queries.GetAll
{
    public record GetAllTaskAnswerQueriesResponse
    {
        public string AnswerText { get; init; }
        public Guid SenderId { get; init; }
    }
}
