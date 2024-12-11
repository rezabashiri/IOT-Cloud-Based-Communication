namespace Shared.Core.Queries;

public interface ICacheable
{
    bool BypassCache { get; }

    string CacheKey { get; }

    TimeSpan? SlidingExpiration { get; }
}