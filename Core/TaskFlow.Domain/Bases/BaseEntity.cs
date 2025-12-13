namespace TaskFlow.Domain.Bases
{
    public abstract class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

    }
}
