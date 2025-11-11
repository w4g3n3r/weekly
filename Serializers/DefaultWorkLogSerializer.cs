using System.Text;
using System.Text.RegularExpressions;
using Weekly.Infrastructure;

namespace Weekly.Serializers
{
    public class DefaultWorkLogSerializer : IWorkLogSerializer
    {
        private const int workLogWeekDayColumnPadding = 8;
        private Dictionary<string, Action<string[], WorkLog>> _lineEvaluators = new Dictionary<string, Action<string[], WorkLog>>();

        private readonly Configuration _configuration;
        private readonly ApiManager _apiManager;


        public DefaultWorkLogSerializer(Configuration configuration, ApiManager apiManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _apiManager = apiManager ?? throw new ArgumentNullException(nameof(apiManager));

            _lineEvaluators.Add(
                @"^(\d{4}-\d{2}-\d{2})\s+(\d{4}-\d{2}-\d{2})",
                ParseRange);

            _lineEvaluators.Add(
                @"^TemplateName\s*=\s*([\w\d_]+)",
                ParseTemplateName);

            _lineEvaluators.Add(
                @"^([^umtwrfsUMTWRFS\W\d_]+)\s*=\s*([\d\.]+)",
                ParseAlias);


            _lineEvaluators.Add(
                @"^([umtwrfsUMTWRFS]+)\s*=\s*([\d\.]+)",
                ParseHoursPerDay);

            _lineEvaluators.Add(
                @"^\s*((?:[umtwrfs]\s*){7})",
                ParseWorkDays);

            _lineEvaluators.Add(
                @"^(?!#)([\S]+)\s+(.*)",
                ParseTimeEntry);
        }

        public string Version => "##wkv1";

        public bool CanDeserialize(string serializedWorkLog)
        {
            return serializedWorkLog.StartsWith(Version);
        }

        public WorkLog Deserialize(string serializedWorkLog)
        {
            var workLog = new WorkLog();
            workLog.Version = Version;

            using (var reader = new StringReader(serializedWorkLog))
            {
                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();

                    foreach (var pattern in _lineEvaluators.Keys)
                    {
                        if (Regex.IsMatch(line, pattern))
                        {
                            var match = Regex.Match(line, pattern);
                            var matches = match.Groups
                                .Cast<Group>()
                                .Select(g => g.Value)
                                .Skip(1).ToArray();

                            _lineEvaluators[pattern](matches, workLog);
                            break;
                        }
                    }
                }
            }
            if (workLog.Period != null)
                workLog.Calculate(_configuration.MinimumHourValue);

            return workLog;
        }

        public string Serialize(WorkLog workLog)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{Version}\n");
            sb.AppendLine($"{workLog.Period}");
            foreach (var alias in workLog.Aliases)
            {
                sb.AppendLine($"{alias.Key} = {alias.Value}");
            }

            foreach (var day in workLog.Period.Days.Keys)
            {
                if (workLog.HoursPerDay.TryGetValue(day, out double hoursInDay) &&
                    hoursInDay != _configuration.HoursPerDay)
                {
                    sb.AppendLine($"{day} = {hoursInDay}");
                }
            }

            sb.AppendLine("\n");

            var maxIssueIdLength = workLog.Any() ? workLog.Max(e => e.IssueKey.Length) + workLogWeekDayColumnPadding : 10;

            int dayCount = 0;
            sb.Append(new string(' ', maxIssueIdLength));
            foreach (var day in workLog.WeekDays)
            {
                if (workLog.Period.Days.ContainsKey(day))
                {
                    sb.Append($" {day} ".PadLeft(workLogWeekDayColumnPadding));
                    dayCount++;
                }
            }
            sb.AppendLine();
            sb.Append("#");
            sb.Append(new string('-', maxIssueIdLength + workLogWeekDayColumnPadding * dayCount));
            sb.AppendLine("#");

            foreach (var issueGroup in workLog.GroupBy(e => e.IssueKey))
            {
                sb.Append(issueGroup.Key.PadRight(maxIssueIdLength));
                foreach (var day in workLog.WeekDays)
                {
                    if (workLog.Period.Days.ContainsKey(day))
                    {
                        var entry = issueGroup.FirstOrDefault(e => e.Date == workLog.Period.GetDate(day));
                        if (entry == null)
                        {
                            sb.Append(" - ".PadLeft(workLogWeekDayColumnPadding));
                        }
                        else
                        {
                            sb.Append($" {entry.Value} ".PadLeft(workLogWeekDayColumnPadding));
                        }
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private void ParseTemplateName(string[] matches, WorkLog workLog)
        {
            workLog.TemplateName = matches[0];
        }

        private void ParseRange(string[] matches, WorkLog workLog)
        {
            try
            {
                var startDate = DateTime.Parse(matches[0]);
                var endDate = DateTime.Parse(matches[1]);

                workLog.ApplyPeriod(new WorkLogPeriod(startDate, endDate), _configuration.HoursPerDay);

            }
            catch (Exception ex)
            {
                throw new FormatException("Invalid date format", ex);
            }
        }

        private void ParseAlias(string[] matches, WorkLog workLog)
        {
            var key = matches[0];
            var value = matches[1];

            if (double.TryParse(value, out double doubleValue))
            {
                workLog.Aliases[key] = doubleValue;
            }
        }

        private void ParseHoursPerDay(string[] matches, WorkLog workLog)
        {
            var key = matches[0];
            var value = matches[1];

            if (double.TryParse(value, out double doubleValue))
            {
                foreach (var c in key)
                {
                    if (WorkLogPeriod.IsValidDayOfWeek(c))
                    {
                        workLog.HoursPerDay[c] = doubleValue;
                    }
                }
            }
        }

        private void ParseWorkDays(string[] matches, WorkLog workLog)
        {
            var workDayHeader = matches[0];
            workLog.WeekDays = workDayHeader
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s[0])
                .ToArray();
        }

        private void ParseTimeEntry(string[] matches, WorkLog workLog)
        {
            var issueId = matches[0];
            var dayValues = matches[1]
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var issue = Task.Run(() => _apiManager.GetIssueAsync(issueId)).Result;

            int dayIndex = 0;

            foreach (char weekDay in workLog.WeekDays)
            {
                var dayValue = dayValues[dayIndex++];
                if (dayValue == "-")
                    continue;

                var entry = new TimeEntry();

                entry.IssueKey = issue.Key;
                entry.IssueId = issue.Id;
                entry.Description = issue.Description;
                entry.Value = dayValue;
                entry.WeekDay = weekDay;

                if (workLog.Period != null)
                {
                    var workDate = workLog.Period.GetDate(weekDay);
                    entry.Date = workDate;
                }

                workLog.Add(entry);
            }
        }
    }
}
