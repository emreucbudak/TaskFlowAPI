namespace TaskFlow.BuildingBlocks.Common
{
    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; }

      
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {
        public BaseEntity()
        {
            Id = Guid.CreateVersion7();
        }
    }
}
