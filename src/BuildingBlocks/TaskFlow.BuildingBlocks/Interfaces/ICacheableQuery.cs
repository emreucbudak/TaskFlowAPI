namespace TaskFlow.BuildingBlocks.Interfaces
{
    public interface ICacheableQuery
    {
        string CacheKey { get;  }    
        TimeSpan? ExpirationTime { get; }
    }
}
