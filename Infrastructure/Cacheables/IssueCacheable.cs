namespace Weekly.Infrastructure.Cacheables
{
    public class IssueCacheable : ICacheable
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string? Description { get; set; }

        public IssueCacheable() { }
        public IssueCacheable(int id, string key, string description)
        {
            Id = id;
            Key = key;
            Description = description;
        }

        public void FromCacheString(string cacheString)
        {
            if (!cacheString.Contains(','))
                throw new ArgumentException("Not a valid cache string.", nameof(cacheString));

            var cacheParts = cacheString.Split(',', StringSplitOptions.TrimEntries);
            Id = int.Parse(cacheParts[0]);
            Key = cacheParts[1];
            Description = cacheParts[2];
        }

        public string ToCacheString()
        {
            return string.Join(",", Id.ToString(), Key, Description);
        }

        public override string ToString()
        {
            return $"[{Key}] {Description}";
        }
    }
}
