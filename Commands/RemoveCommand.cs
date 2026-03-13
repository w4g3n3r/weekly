using Spectre.Console;
using System.CommandLine;
using System.Text.RegularExpressions;
using Weekly.Exceptions;
using Weekly.Services;

namespace Weekly.Commands
{
    public class RemoveCommand : BaseCommand
    {
        private const string DayOfWeekPattern = "^[smtwrfu]$";
        private const string IssueIdPattern = @"^\w+\-\d+$";
        private const string CommandName = "remove";
        private const string CommandDescription = "Remove a time entry from a worklog.";


        public RemoveCommand(
            WeeklyService weeklyService,
            Configuration configuration)
            : base(weeklyService, configuration, CommandName, CommandDescription)
        {
            var workLogOption = new Option<string>(
                new string[] { "--workLog", "-w" },
                description: "The file name of the work log to remove the time entry from. Use 'p' for previous period. If not provided, the current work log will be used."
            );
            var dayOfWeekArgument = new Argument<string>(
                "day",
                description: "The day of the week to remove the time entry from. If not provided, the current day will be used."
            );
            var issueIdArgument = new Argument<string>(
                "issueId",
                description: "The issue ID to remove from the work log."
            );

            dayOfWeekArgument.Arity = ArgumentArity.ZeroOrOne;
            issueIdArgument.Arity = ArgumentArity.ZeroOrOne;

            AddAlias("rm");

            AddOption(workLogOption);
            AddArgument(dayOfWeekArgument);
            AddArgument(issueIdArgument);

            this.SetHandler(Remove, workLogOption, dayOfWeekArgument, issueIdArgument);
        }

        public async Task Remove(string workLogName, string dayOfWeek, string issueKey)
        {
            try
            {
                (dayOfWeek, issueKey) = ParseArguments(dayOfWeek, issueKey);

                var workLog = await _weeklyService.GetWorkLogAsync(workLogName);

                var timeEntry = workLog.RemoveTimeEntry(issueKey, dayOfWeek);                
                workLog.Calculate(_configuration.MinimumHourValue);

                await _weeklyService.SaveWorkLogAsync(workLog);

                var dayOfWeekName = WorkLogPeriod.GetDayOfWeek(timeEntry.WeekDay);

                WriteStatus($"Removed [yellow]{issueKey}[/] from [yellow]{dayOfWeekName}[/].");
                WriteStatus(timeEntry.ToString().EscapeMarkup());

            }
            catch (ProgramException ex)
            {
                WriteProgramExceptionAndExit(ex);
            }

        }

        private (string, string) ParseArguments(params string[] args)
        {
            var validArgs = args.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var dayOfWeekRegEx = new Regex(DayOfWeekPattern);
            var dayOfWeek = validArgs.Where(x => dayOfWeekRegEx.IsMatch(x))
                .FirstOrDefault() ?? WorkLogPeriod.GetDayOfWeek(DateTime.Now.DayOfWeek).ToString();

            var issueIdRegEx = new Regex(IssueIdPattern);
            var issueId = validArgs.Where(x => issueIdRegEx.IsMatch(x))
                .FirstOrDefault() ?? throw new InputException("An Issue ID is required, make sure it matches this format: [ProjectKey]-[IssueNumber]");

            return (dayOfWeek, issueId);
        }
    }
}
