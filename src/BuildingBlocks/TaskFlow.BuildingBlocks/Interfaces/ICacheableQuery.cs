namespace TaskFlow.BuildingBlocks.Interfaces
{
    public class ICacheableQuery
    {
        string CacheKey { get;  }    
        TimeSpan? ExpirationTime { get; }
    }
}
