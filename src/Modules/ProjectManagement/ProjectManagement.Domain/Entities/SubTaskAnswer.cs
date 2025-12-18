using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class SubTaskAnswer : BaseEntity
    {
        public SubTaskAnswer(string answerText, Guid senderId)
        {
            if (string.IsNullOrEmpty(answerText)) {
                throw new Exception("AnswerText boş veya null olamaz");
            }
            AnswerText = answerText;
            SenderId = senderId;
        }

        public string AnswerText { get; private set; }
        public Guid SenderId { get; private set; }
    }
}
