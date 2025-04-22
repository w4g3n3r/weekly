using System.Text.RegularExpressions;

namespace Weekly
{
    public class WorkLog : List<TimeEntry>
    {
        public WorkLog()
        {
            this.HoursPerDay = new Dictionary<char, double>();
            this.Aliases = new Dictionary<string, double>();
            this.WeekDays = new char[0];
        }

        public string TemplateName { get; set; }
        public char[] WeekDays { get; set; }
        public WorkLogPeriod Period { get; private set; }
        public Dictionary<char, double> HoursPerDay { get; set; }
        public Dictionary<string, double> Aliases { get; set; }
        public string FilePath { get; set; }
        public string Version { get; set; }


        public void Calculate(double minimumValue)
        {
            this.ForEach(e => e.Calculate(Aliases, minimumValue));

            CalculateDynamicHours(minimumValue);
        }

        public TimeEntry AddTimeEntry(
            string issueKey,
            string weekDay,
            string value,
            int issueId,
            string issueDescription)
        {
            var timeEntry = this
                .FirstOrDefault(t => t.IssueKey.Equals(issueKey, StringComparison.OrdinalIgnoreCase) &&
                    t.WeekDay.ToString().Equals(weekDay.ToString(), StringComparison.OrdinalIgnoreCase));

            var dynamicRegex = new Regex(@"^\**$");

            if (timeEntry == null)
            {
                timeEntry = new TimeEntry
                {
                    IssueKey = issueKey,
                    WeekDay = weekDay[0],
                    Value = value,
                    Date = Period.GetDate(weekDay[0]),
                    IssueId = issueId,
                    Description = issueDescription
                };

                this.Add(timeEntry);
            }
            else
            {
                if (double.TryParse(value, out double d))
                {
                    d += timeEntry.Hours;
                    timeEntry.Value = d.ToString("0.00");
                    timeEntry.Hours = d;
                }
                else if (dynamicRegex.IsMatch(value))
                {
                    timeEntry.Hours = 0;
                    if (timeEntry.IsDynamic)
                    {
                        timeEntry.Value += value;
                    }
                    else
                    {
                        timeEntry.Value = value;
                    }
                }
                else
                {
                    timeEntry.Hours = 0;
                    if (timeEntry.IsDynamic)
                    {
                        timeEntry.Value = value;
                    }
                    else
                    {
                        timeEntry.Value += value;
                    }
                }

                timeEntry.IssueKey = issueKey;
                timeEntry.IssueId = issueId;
                timeEntry.Description = issueDescription;
            }

            return timeEntry;
        }

        public void ApplyPeriod(WorkLogPeriod period, double defaultHoursPerDay)
        {
            var issuesByIssueId = this
                .GroupBy(t => t.IssueKey)
                .ToDictionary(g => g.Key, g => g.ToList());

            this.RemoveAll(t => !period.Days.ContainsKey(t.WeekDay));

            foreach (var issueId in issuesByIssueId.Keys)
            {
                var issues = issuesByIssueId[issueId];

                foreach (var issue in issues)
                {
                    if (period.Days.ContainsKey(issue.WeekDay))
                    {
                        issue.Date = period.GetDate(issue.WeekDay);
                    }
                }
            }

            this.WeekDays = period.Days
                .Select(p => new { WeekDay = p.Key, WeekDate = p.Value })
                .OrderBy(p => p.WeekDate)
                .Select(p => p.WeekDay)
                .ToArray();

            foreach (var day in period.Days.Keys)
            {
                if (!HoursPerDay.ContainsKey(day))
                {
                    HoursPerDay[day] = defaultHoursPerDay;
                }
            }

            foreach (var day in this.HoursPerDay.Keys)
            {
                if (!period.Days.ContainsKey(day))
                {
                    HoursPerDay.Remove(day);
                }
            }

            this.Period = period;
        }

        private void CalculateDynamicHours(double minimumValue)
        {
            foreach (var day in WeekDays)
            {
                var workDate = Period.GetDate(day);

                var maxHours = HoursPerDay[day];
                var hours = this
                    .Where(e => e.Date == workDate)
                    .Select(e => e.Hours)
                    .Sum();
                var totalMultipliers = this
                    .Where(e => e.Date == workDate)
                    .Select(e => e.DynamicMultiplier)
                    .Sum();

                var dynamicValue = (maxHours - hours) / totalMultipliers;

                this
                    .Where(e => e.Date == workDate && e.IsDynamic)
                    .ToList()
                    .ForEach(e =>
                    {
                        e.Hours = Math.Max(e.DynamicMultiplier * dynamicValue, minimumValue);
                    });
            }
        }
    }
}
