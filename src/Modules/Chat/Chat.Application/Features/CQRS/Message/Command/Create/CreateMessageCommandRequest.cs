using FlashMediator;

namespace Chat.Application.Features.CQRS.Message.Command.Create
{
    public record CreateMessageCommandRequest : IRequest
    {
        public string Content { get; init; }
        public Guid SenderId { get; init; }
        public Guid? ReceiverId { get; init; }
        public Guid? GroupId { get; init; }

        public CreateMessageCommandRequest(string content, Guid senderId, Guid? receiverId, Guid? groupId)
        {
            Content = content;
            SenderId = senderId;
            ReceiverId = receiverId;
            GroupId = groupId;
        }
    }
}
