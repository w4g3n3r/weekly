namespace Weekly.Infrastructure.Cacheables
{
    public interface ICacheable
    {
        string ToCacheString();
        void FromCacheString(string cacheString);
    }
}
