using TaskFlow.BuildingBlocks.Common;

namespace Identity.Domain.Entities;

public class GroupEvent : BaseEntity
{
    public string Subject { get; private set; }
    public string EventType { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid GroupId { get; private set; }
    public Groups Group { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public User CreatedByUser { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime? EndsAt { get; private set; }
    public string? MeetingLink { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private GroupEvent()
    {
        Subject = string.Empty;
        EventType = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
    }

    public GroupEvent(
        string subject,
        string eventType,
        string title,
        string description,
        DateTime startsAt,
        DateTime? endsAt,
        string? meetingLink,
        Guid groupId,
        Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Etkinlik konusu bos olamaz.");
        }

        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("Etkinlik turu bos olamaz.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Etkinlik basligi bos olamaz.");
        }

        if (startsAt == default)
        {
            throw new ArgumentException("Etkinlik baslangic zamani bos olamaz.");
        }

        if (endsAt.HasValue && endsAt.Value <= startsAt)
        {
            throw new ArgumentException("Etkinlik bitis zamani baslangic zamanindan sonra olmali.");
        }

        Subject = subject.Trim();
        EventType = eventType.Trim();
        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        StartsAt = DateTime.SpecifyKind(startsAt, DateTimeKind.Utc);
        EndsAt = endsAt.HasValue ? DateTime.SpecifyKind(endsAt.Value, DateTimeKind.Utc) : null;
        MeetingLink = string.IsNullOrWhiteSpace(meetingLink) ? null : meetingLink.Trim();
        GroupId = groupId;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }
}
