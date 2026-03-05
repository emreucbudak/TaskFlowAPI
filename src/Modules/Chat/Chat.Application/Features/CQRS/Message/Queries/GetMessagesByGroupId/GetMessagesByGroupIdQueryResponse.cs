namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesByGroupId
{
    public record GetMessagesByGroupIdQueryResponse
    {
        public Guid Id { get; init; }
        public string Content { get; init; }
        public bool IsRead { get; init; }
        public DateTime SendTime { get; init; }
        public Guid SenderId { get; init; }
        public Guid? ReceiverId { get; init; }
        public Guid? GroupId { get; init; }
        public bool IsDeleted { get; init; }
        public bool IsEdited { get; init; }
        public bool IsDelivered { get; init; }
        public DateTime? DeliveredTime { get; init; }
    }
}
