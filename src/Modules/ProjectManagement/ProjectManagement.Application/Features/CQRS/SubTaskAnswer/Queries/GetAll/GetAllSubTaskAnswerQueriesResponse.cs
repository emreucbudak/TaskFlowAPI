namespace ProjectManagement.Application.Features.CQRS.SubTaskAnswer.Queries.GetAll
{
    public record GetAllSubTaskAnswerQueriesResponse
    {
        public string AnswerText { get;  init; }
        public Guid SenderId { get; init; }
    }
}
