using Spectre.Console;
using Spectre.Console.Rendering;
using System.CommandLine;
using Weekly.Exceptions;
using Weekly.Services;

namespace Weekly.Commands
{
    public class StatusCommand : BaseCommand
    {
        private const string CommandName = "status";
        private const string CommandDescription = "Show the status of a worklog.";

        public StatusCommand(
            WeeklyService weeklyService,
            Configuration configuration)
            : base(weeklyService, configuration, CommandName, CommandDescription)
        {
            var workLogOption = new Option<string>(
                new string[] { "--workLog", "-w" },
                description: "The file name of the work log to show the status of. Use 'p' for previous period. If not provided, the current work log will be shown."
            );

            AddAlias("s");

            AddOption(workLogOption);
            this.SetHandler(Status, workLogOption);
        }

        public async Task Status(string workLogName)
        {
            try
            {
                WorkLog workLog = await _weeklyService.GetWorkLogAsync(workLogName);
                var maxWidth = Console.WindowWidth;

                var issueGroup = workLog
                    .GroupBy(e => e.IssueKey)
                    .ToDictionary(g => g.Key, g => g.OrderBy(e => e.Date).ToList());

                var workLogTable = new Table()
                    .Border(TableBorder.DoubleEdge)
                    .BorderColor(Color.Grey23)
                    .Title($"Work Log: {workLog.Period.ToLongString()}")
                    .Caption($"Weekly Total: {workLog.Select(e => e.Hours).Sum():0.00}");

                workLogTable.AddColumn(new TableColumn("[olive]Issue ID[/]")
                    .Footer("[teal]Totals[/]")
                    .Padding(1, 0)
                    .LeftAligned());

                foreach (var day in workLog.Period.Days.Keys)
                {
                    var currentDate = workLog.Period.GetDate(day);
                    if (!currentDate.HasValue)
                        continue;

                    var dayTotal = workLog
                        .Where(e => e.Date == currentDate)
                        .Sum(e => e.Hours);

                    workLogTable.AddColumn(new TableColumn($"[olive]{currentDate.Value.ToString("ddd")}[/]")
                        .Footer($"[teal]{dayTotal.ToString("0.00")}[/]")
                        .Padding(1, 0)
                        .RightAligned()
                        .NoWrap());
                }

                foreach (var issue in issueGroup.Keys.Order(new Extensions.NaturalStringComparer()))
                {
                    var entries = issueGroup[issue];
                    var rowItems = new List<IRenderable>();

                    string description = entries.FirstOrDefault()?.Description ?? issue;

                    if (maxWidth < 63)
                    {
                        rowItems.Add(new Text(issue));
                    }
                    else
                    {
                        var columns = new Columns(
                            new Text(issue)
                                .Justify(Justify.Left),
                            new Padder(
                                new Markup($"[dim]{description.EscapeMarkup()}[/]")
                                    .Justify(Justify.Left)
                                    .Crop())
                                .Padding(1, 0, 0, 0))
                            .Padding(1, 0, 0, 0)
                            .Collapse();

                        rowItems.Add(columns);
                    }

                    foreach (var day in workLog.Period.Days.Keys)
                    {
                        var dayEntries = entries.Where(e => e.Date == workLog.Period.GetDate(day)).ToList();
                        if (dayEntries.Count == 0)
                        {
                            rowItems.Add(new Markup("[dim]----[/]"));
                            continue;
                        }
                        else
                        {
                            rowItems.Add(new Text(dayEntries.First().Hours.ToString("0.00")));
                        }
                    }
                    workLogTable.AddRow(rowItems);
                }

                var padding = new Padder(workLogTable)
                    .Padding(3, 1);

                AnsiConsole.Write(padding);
            }
            catch (ProgramException pe)
            {
                WriteProgramExceptionAndExit(pe);
            }
        }
    }
}
