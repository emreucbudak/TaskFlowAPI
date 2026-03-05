using ProjectManagement.Domain.Exceptions;
using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Domain.Entities
{
    public class SubTaskAnswer : BaseEntity
    {
        public SubTaskAnswer(string answerText, Guid senderId)
        {
            if (string.IsNullOrWhiteSpace(answerText))
            {
                throw new TaskDomainException("Answer text bos veya null olamaz");
            }

            AnswerText = answerText;
            SenderId = senderId;
        }

        public string AnswerText { get; private set; }
        public Guid SenderId { get; private set; }

        public void UpdateAnswerText(string answerText)
        {
            AnswerText = answerText;
        }
    }
}
