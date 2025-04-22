namespace Weekly.Infrastructure.Cacheables
{
    public class AccountCacheable : ICacheable
    {
        public string AccountId { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }

        public AccountCacheable() { }

        public AccountCacheable(string accountId, string displayName, string emailAddress)
        {
            AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        }

        public void FromCacheString(string cacheString)
        {
            if (!cacheString.Contains(','))
                throw new ArgumentException("Not a valid cache string.", nameof(cacheString));

            var cacheParts = cacheString.Split(',', StringSplitOptions.TrimEntries);

            AccountId = cacheParts[0];
            DisplayName = cacheParts[1];
            EmailAddress = cacheParts[2];
        }

        public string ToCacheString()
        {
            return string.Join(",", AccountId, DisplayName, EmailAddress);
        }
    }
}
