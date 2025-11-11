using Flurl;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Weekly.Exceptions;
using Weekly.Infrastructure.Cacheables;

namespace Weekly.Infrastructure
{
    public class ApiManager
    {
        private const string JiraApi = "jira";
        private const string TempoApi = "tempo";

        private readonly CacheManager _cacheManager;
        private readonly TokenManager _tokenManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiManager(
            CacheManager cacheManager,
            TokenManager tokenManager,
            IHttpClientFactory httpClientFactory)
        {
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IssueCacheable> GetIssueAsync(string issueKey)
        {
            var cacheKey = $"issuekey.{issueKey}";

            if (_cacheManager.TryGetValue<IssueCacheable>(cacheKey, out var cachedIssue))
                return cachedIssue;

            var httpClient = GetJiraHttpClient();

            var issue = await httpClient.GetFromJsonAsync<JiraIssue>($"issue/{issueKey}?fields=summary");

            if (issue == null)
                return null;

            cachedIssue = new IssueCacheable(issue.Id, issue.Key, issue.Fields.Summary);
            _cacheManager.SetValue(cacheKey, cachedIssue);

            return cachedIssue;
        }

        public async Task<string> GetAccountIdAsync(bool bypassCache = false)
        {
            var cacheKey = "account.myself";

            if (!bypassCache && _cacheManager.TryGetValue<AccountCacheable>(cacheKey, out var cachedAccount))
                return cachedAccount.AccountId;

            var httpClient = GetJiraHttpClient();

            var account = await httpClient.GetFromJsonAsync<JiraAccount>("myself");

            if (account == null)
                return null;

            cachedAccount = new AccountCacheable(account.AccountId, account.DisplayName, account.EmailAddress);
            _cacheManager.SetValue<AccountCacheable>(cacheKey, cachedAccount);

            return account.AccountId;
        }

        public async Task<bool> TryPostTimeEntryAsync(TimeEntry entry, string accountId)
        {
            var httpClient = GetTempoHttpClient();

            var post = new TempoWorkLog
            {
                StartDate = entry.Date.Value.ToString("yyyy-MM-dd"),
                AuthorAccountId = accountId,
                Description = $"{entry.IssueKey} · {entry.Description}",
                IssueId = entry.IssueId,
                TimeSpentSeconds = (int)(entry.Hours * 60 * 60)
            };

            try
            {
                var response = await httpClient.PostAsJsonAsync("worklogs", post);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async IAsyncEnumerable<TimeEntry> GetTimeEntriesAsync(DateTime start, DateTime end)
        {
            var httpClient = GetTempoHttpClient();

            var accountId = await GetAccountIdAsync();

            var url = $"worklogs/user/{accountId}"
                .SetQueryParam("from", start.ToString("yyyy-MM-dd"))
                .SetQueryParam("to", end.ToString("yyyy-MM-dd"));

            var response = await httpClient.GetFromJsonAsync<TempoWorklogResponse>(url);

            if (response != null)
            {
                foreach (var workLog in response.Results)
                {
                    yield return new TimeEntry
                    {
                        Date = DateTime.Parse(workLog.StartDate),
                        Description = workLog.Description,
                        IssueId = workLog.Issue.Id,
                        Hours = (double)workLog.TimeSpentSeconds / 60.0 / 60.0,
                        ExternalId = workLog.TempoWorklogId
                    };
                }
            }
            else
            {
                yield break;
            }
        }

        public async Task<bool> TryDeleteTimeEntryAsync(int externalId)
        {
            var httpClient = GetTempoHttpClient();

            var url = $"worklogs/{externalId}"
                .SetQueryParam("bypassPeriodClosuresAndApprovals", false);

            try
            {
                var response = await httpClient.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private HttpClient GetTempoHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient(TempoApi);

            if (!_tokenManager.TokenExists(TempoApi))
            {
                throw new MissingTokenException(TempoApi);
            }

            var authHeader = new AuthenticationHeaderValue("Bearer", _tokenManager.GetBearerAuthToken(TempoApi));
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            return httpClient;
        }

        private HttpClient GetJiraHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient(JiraApi);

            if (!_tokenManager.TokenExists(JiraApi))
            {
                throw new MissingTokenException(JiraApi);
            }

            var authHeader = new AuthenticationHeaderValue("Basic", $"{_tokenManager.GetBasicAuthToken(JiraApi)}");
            httpClient.DefaultRequestHeaders.Authorization = authHeader;

            return httpClient;
        }

        [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
        private class JiraIssue
        {
            public class JiraIssueFields
            {
                public string Summary { get; set; }
            }

            public int Id { get; set; }
            public string Key { get; set; }
            public string Summary { get; set; }
            public JiraIssueFields Fields { get; set; }
        }

        [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
        private class JiraAccount
        {
            public string AccountId { get; set; }
            public string DisplayName { get; set; }
            public string EmailAddress { get; set; }
        }

        [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
        private class TempoWorkLog
        {
            public string AuthorAccountId { get; set; }
            public int TimeSpentSeconds { get; set; }
            public string StartDate { get; set; }
            public int IssueId { get; set; }
            public string Description { get; set; }
        }

        [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
        private class TempoWorkLogCollection
        {
            public TempoWorkLog[] Ressults { get; set; }
        }


        public class TempoWorklogResponse
        {
            public string Self { get; set; }
            public TempoMetadata Metadata { get; set; }
            public List<TempoWorklog> Results { get; set; }
        }

        public class TempoMetadata
        {
            public int Count { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
            public string Next { get; set; }
        }

        public class TempoWorklog
        {
            public string Self { get; set; }
            public int TempoWorklogId { get; set; }
            public TempoIssue Issue { get; set; }
            public int TimeSpentSeconds { get; set; }
            public int BillableSeconds { get; set; }
            public string StartDate { get; set; }      // Consider DateOnly if using .NET 6+
            public string StartTime { get; set; }      // Consider TimeOnly if using .NET 6+
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public TempoAuthor Author { get; set; }
            public TempoAttributes Attributes { get; set; }
        }

        public class TempoIssue
        {
            public string Self { get; set; }
            public int Id { get; set; }
        }

        public class TempoAuthor
        {
            public string Self { get; set; }
            public string AccountId { get; set; }
        }

        public class TempoAttributes
        {
            public string Self { get; set; }
            public List<object> Values { get; set; } = new(); // Can be replaced with a concrete class if values have structure
        }
    }
}
