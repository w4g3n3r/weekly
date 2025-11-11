using System.Collections.Concurrent;
using Weekly.Exceptions;
using Weekly.Infrastructure.Cacheables;

namespace Weekly.Services
{
    public partial class WeeklyService
    {
        public async Task<IssueCacheable> GetIssueAsync(string issueKey)
        {
            try
            {
                return await _apiManager.GetIssueAsync(issueKey);
            }
            catch (HttpRequestException ex)
            {
                // Jira's issue API returns a 404 if your're unauthenticated. 
                // We can verify that it's not a 401 by hitting the account
                // API.
                try
                {
                    _ = await _apiManager.GetAccountIdAsync(true);
                }
                catch (HttpRequestException hre) when (hre.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new ApiUnauthorizedException("Jira", hre);
                }

                throw new IssueNotFoundException(issueKey, ex);

            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        public async IAsyncEnumerable<TimeEntryResult> PushWorkLogAsync(WorkLog workLog)
        {
            var accountId = await _apiManager.GetAccountIdAsync();

            if (accountId == null)
                throw new ApiException("Jira");


            using (BlockingCollection<TimeEntryResult> results = new BlockingCollection<TimeEntryResult>())
            {
                var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                var loop = Parallel.ForEachAsync(workLog, options, async (timeEntry, token) =>
                {
                    var success = await _apiManager.TryPostTimeEntryAsync(timeEntry, accountId);

                    results.Add(new TimeEntryResult { TimeEntry = timeEntry, IsSuccessful = success });

                }).ContinueWith(t => results.CompleteAdding());

                foreach (var result in results.GetConsumingEnumerable())
                {
                    yield return result;
                }

                await loop;
            }

        }

        public async IAsyncEnumerable<TimeEntryResult> DeleteTimeEntriesForPeriod(WorkLogPeriod period)
        {

            using (BlockingCollection<TimeEntryResult> results = new BlockingCollection<TimeEntryResult>())
            {
                var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

                var loop = Parallel.ForEachAsync(
                    _apiManager.GetTimeEntriesAsync(period.StartDate, period.EndDate),
                    options,
                    async (timeEntry, token) =>
                {
                    var success = await _apiManager.TryDeleteTimeEntryAsync(timeEntry.ExternalId);

                    results.Add(new TimeEntryResult { TimeEntry = timeEntry, IsSuccessful = success });

                }).ContinueWith(t => results.CompleteAdding());

                foreach (var result in results.GetConsumingEnumerable())
                {
                    yield return result;
                }

                await loop;
            }
        }

        public async Task<bool> PeriodHasTimeEntries(WorkLogPeriod period)
        {
            try
            {
                var hasTimeEntries = await _apiManager.GetTimeEntriesAsync(period.StartDate, period.EndDate)
                    .AnyAsync();

                return hasTimeEntries;
            }
            catch (HttpRequestException rex) when (rex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new ApiUnauthorizedException("Jira", rex);
            }
            catch (HttpRequestException rex)
            {
                throw new ApiException("Jira", rex);
            }
            catch (Exception ex) when (ex is not ProgramException)
            {
                throw new InternalException(ex);
            }
        }

        public async IAsyncEnumerable<TimeEntry> GetWorkLogsAsync(DateTime start, DateTime end)
        {
            await foreach (var entry in _apiManager.GetTimeEntriesAsync(start, end))
            {
                yield return entry;
            }
        }
    }
}
