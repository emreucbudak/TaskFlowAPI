using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class TaskAnswer : BaseEntity
    {
        public TaskAnswer(string answerText, Guid senderId, Guid taskId)
        {
            if (string.IsNullOrWhiteSpace(answerText))
            {
                throw new Exception("Answer Text Boş Gönderilemez");
            }
            AnswerText = answerText;
            SenderId = senderId;
            TaskId = taskId;
            this.CreatedDate = DateTime.UtcNow;
        }
        protected TaskAnswer(){}
        public string AnswerText { get; private set; }
        public Guid SenderId { get; private set; }
        public Guid TaskId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public void UpdateAnswerText(string answerText)
        {
            this.AnswerText = answerText;
        }


    }
}
