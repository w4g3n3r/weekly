using System.Text;

namespace Weekly
{
    public class TimeEntryResult
    {
        public TimeEntry TimeEntry { get; set; }
        public bool IsSuccessful { get; set; }

        public string ToMarkupString()
        {
            var sb = new StringBuilder();
            sb.Append(this.IsSuccessful ? "[green]√[/]" : "[red]X[/]");
            sb.Append(" ");
            sb.Append(TimeEntry.ToString());

            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.IsSuccessful ? "√" : "X");
            sb.Append(" ");
            sb.Append(TimeEntry.ToString());

            return sb.ToString();
        }
    }
}
