using System.Text;

namespace Weekly
{
    public class TimeEntry
    {
        public DateTime? Date { get; set; }
        public string IssueKey { get; set; }
        public int IssueId { get; set; }
        public string Description { get; set; }
        public double Hours { get; set; }
        public char WeekDay { get; set; }
        public string Value { get; set; }
        public int ExternalId { get; set; }
        public bool IsDynamic => Value.Contains("*");
        public int DynamicMultiplier => Value.Count(c => c == '*');

        public void Calculate(Dictionary<string, double> aliases, double minimumValue)
        {
            if (this.IsDynamic)
                return;

            var value = Value;
            var valueLength = value.Length;
            var values = new List<double>();

            foreach (var key in aliases.Keys)
            {
                value = value.Replace(key, string.Empty);
                var aliasCount = (valueLength - value.Length) / key.Length;

                if (aliasCount > 0)
                {
                    values.Add(aliasCount * aliases[key]);
                }

                valueLength = value.Length;
            }

            if (double.TryParse(value, out double doubleValue))
            {
                values.Add(doubleValue);
            }

            Hours = Math.Max(values.Sum(), minimumValue);
        }

        public override string ToString()
        {

            var sb = new StringBuilder();
            sb.Append(Date.HasValue ? Date.Value.ToString("ddd - ") : string.Empty);
            sb.Append($"{Hours:0.00}h - ");

            if (!string.IsNullOrEmpty(IssueKey))
            {
                sb.Append($"{IssueKey}: ");
            }

            sb.Append(Description);

            return sb.ToString();
        }
    }
}
