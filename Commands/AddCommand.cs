using Spectre.Console;
using System.CommandLine;
using System.Text.RegularExpressions;
using Weekly.Exceptions;
using Weekly.Services;

namespace Weekly.Commands
{
    public class AddCommand : BaseCommand
    {
        private const string DayOfWeekPattern = "^[smtwrfu]$";
        private const string IssueIdPattern = @"^\w+\-\d+$";
        private const string CommandName = "add";
        private const string CommandDescription = "Add a time entry to a worklog.";


        public AddCommand(
            WeeklyService weeklyService,
            Configuration configuration)
            : base(weeklyService, configuration, CommandName, CommandDescription)
        {
            var workLogOption = new Option<string>(
                new string[] { "--workLog", "-w" },
                description: "The file name of the work log to add the time entry to. If not provided, the current work log will be used."
            );
            var templateOption = new Option<string>(
                new string[] { "--template", "-t" },
                description: "The template to use when creating a new work log. If not provided, the default template will be used."
            );
            var dayOfWeekArgument = new Argument<string>(
                "day",
                description: "The day of the week to add the time entry to. If not provided, the current day will be used."
            );
            var issueIdArgument = new Argument<string>(
                "issueId",
                description: "The issue ID to add the time entry to."
            );
            var hoursArgument = new Argument<string>(
                "hours",
                description: "The number of hours to add to the time entry. If not provided, the MinimumHourValue from your config will be used."
            );

            dayOfWeekArgument.Arity = ArgumentArity.ZeroOrOne;
            issueIdArgument.Arity = ArgumentArity.ZeroOrOne;
            hoursArgument.Arity = ArgumentArity.ZeroOrOne;

            AddAlias("a");

            AddOption(workLogOption);
            AddOption(templateOption);
            AddArgument(dayOfWeekArgument);
            AddArgument(issueIdArgument);
            AddArgument(hoursArgument);

            this.SetHandler(Add, workLogOption, templateOption, dayOfWeekArgument, issueIdArgument, hoursArgument);
        }

        public async Task Add(string workLogName, string templateName, string dayOfWeek, string issueKey, string hours)
        {
            try
            {
                (dayOfWeek, issueKey, hours) = ParseArguments(dayOfWeek, issueKey, hours);

                var issue = await _weeklyService.GetIssueAsync(issueKey);

                var workLog = await _weeklyService.GetOrCreateWorkLogAsync(workLogName, templateName);

                var timeEntry = workLog.AddTimeEntry(issue.Key, dayOfWeek, hours, issue.Id, issue.Description);
                workLog.Calculate(_configuration.MinimumHourValue);

                await _weeklyService.SaveWorkLogAsync(workLog);

                WriteStatus($"Added [yellow]{hours}[/] hour(s) to [yellow]{issueKey}[/].");
                WriteStatus(timeEntry.ToString().EscapeMarkup());

            }
            catch (ProgramException ex)
            {
                WriteProgramExceptionAndExit(ex);
            }

        }

        private (string, string, string) ParseArguments(params string[] args)
        {
            var validArgs = args.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var dayOfWeekRegEx = new Regex(DayOfWeekPattern);
            var dayOfWeek = validArgs.Where(x => dayOfWeekRegEx.IsMatch(x))
                .FirstOrDefault() ?? WorkLogPeriod.GetDayOfWeek(DateTime.Now.DayOfWeek).ToString();

            var issueIdRegEx = new Regex(IssueIdPattern);
            var issueId = validArgs.Where(x => issueIdRegEx.IsMatch(x))
                .FirstOrDefault() ?? throw new InputException("An Issue ID is required, make sure it matches this format: [ProjectKey]-[IssueNumber]");

            var hours = validArgs.Where(x => x != dayOfWeek && x != issueId)
                .FirstOrDefault() ?? _configuration.MinimumHourValue.ToString();

            return (dayOfWeek, issueId, hours);
        }
    }
}
