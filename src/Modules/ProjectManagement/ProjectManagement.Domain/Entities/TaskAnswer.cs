using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class TaskAnswer : BaseEntity
    {
        public TaskAnswer(string answerText, Guid senderId)
        {
            if (string.IsNullOrWhiteSpace(answerText))
            {
                throw new Exception("Answer Text Boş Gönderilemez");
            }
            AnswerText = answerText;
            SenderId = senderId;
        }
        protected TaskAnswer(){}
        public string AnswerText { get; private set; }
        public Guid SenderId { get; private set; }


    }
}
