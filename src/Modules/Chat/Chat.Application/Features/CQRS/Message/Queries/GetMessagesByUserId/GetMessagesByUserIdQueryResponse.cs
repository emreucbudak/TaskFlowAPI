namespace Chat.Application.Features.CQRS.Message.Queries.GetMessagesByUserId
{
    public record GetMessagesByUserIdQueryResponse
    {
        public Guid Id { get; init; }
        public string Content { get; init; }
        public bool IsRead { get; init; }
        public DateTime SendTime { get; init; }
        public Guid SenderId { get; init; }
        public Guid? ReceiverId { get; init; }
        public Guid? GroupId { get; init; }
        public bool isDeleted { get; init; }
        public bool isEdited { get; init; }
        public bool isDelivered { get; init; }
        public DateTime? DeliveredTime { get; init; }
    }
}
