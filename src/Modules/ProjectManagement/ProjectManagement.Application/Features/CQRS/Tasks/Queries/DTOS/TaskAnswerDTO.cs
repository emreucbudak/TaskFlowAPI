namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries.DTOS
{
    public record TaskAnswerDTO
    {
        public string AnswerText { get; init; }
        public Guid SenderId { get; init; }
    }
}
