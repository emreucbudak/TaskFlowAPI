using TaskFlow.BuildingBlocks.Common;

namespace Identity.Domain.Entities
{
    public class GroupActivity : BaseEntity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid GroupId { get; private set; }
        public Groups Group { get; private set; }
        public Guid SubmittedByUserId { get; private set; }
        public User SubmittedByUser { get; private set; }
        public DateTime SubmittedAt { get; private set; }
        public GroupActivityStatus Status { get; private set; }
        public Guid? ReviewedByUserId { get; private set; }
        public User? ReviewedByUser { get; private set; }
        public DateTime? ReviewedAt { get; private set; }
        public string? ReviewNote { get; private set; }

        private GroupActivity() { }

        public GroupActivity(string title, string description, Guid groupId, Guid submittedByUserId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Aktivite basligi bos olamaz.");

            Title = title;
            Description = description ?? string.Empty;
            GroupId = groupId;
            SubmittedByUserId = submittedByUserId;
            SubmittedAt = DateTime.UtcNow;
            Status = GroupActivityStatus.Pending;
        }

        public void Approve(Guid reviewedByUserId, string? note = null)
        {
            if (Status != GroupActivityStatus.Pending)
                throw new InvalidOperationException("Sadece bekleyen aktiviteler onaylanabilir.");

            Status = GroupActivityStatus.Approved;
            ReviewedByUserId = reviewedByUserId;
            ReviewedAt = DateTime.UtcNow;
            ReviewNote = note;
        }

        public void Reject(Guid reviewedByUserId, string? note = null)
        {
            if (Status != GroupActivityStatus.Pending)
                throw new InvalidOperationException("Sadece bekleyen aktiviteler reddedilebilir.");

            Status = GroupActivityStatus.Rejected;
            ReviewedByUserId = reviewedByUserId;
            ReviewedAt = DateTime.UtcNow;
            ReviewNote = note;
        }
    }

    public enum GroupActivityStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
