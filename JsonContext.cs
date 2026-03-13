using System.Text.Json.Serialization;
using Weekly.Infrastructure;

namespace Weekly
{
    [JsonSerializable(typeof(Configuration))]
    [JsonSerializable(typeof(ApiManager.JiraIssue))]
    [JsonSerializable(typeof(ApiManager.JiraIssue.JiraIssueFields))]
    [JsonSerializable(typeof(ApiManager.JiraAccount))]
    [JsonSerializable(typeof(ApiManager.TempoWorkLogPost))]
    [JsonSerializable(typeof(ApiManager.TempoWorklogResponse))]
    [JsonSerializable(typeof(ApiManager.TempoMetadata))]
    [JsonSerializable(typeof(ApiManager.TempoWorklog))]
    [JsonSerializable(typeof(ApiManager.TempoIssue))]
    [JsonSerializable(typeof(ApiManager.TempoAuthor))]
    [JsonSerializable(typeof(ApiManager.TempoAttributes))]
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString)]
    internal partial class WeeklyJsonContext : JsonSerializerContext
    {
    }
}
