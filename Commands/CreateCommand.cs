using System.CommandLine;
using Weekly.Exceptions;
using Weekly.Services;

namespace Weekly.Commands
{
    public class CreateCommand : BaseCommand
    {
        private const string CommandName = "create";
        private const string CommandDescription = "Create a new work log for a selected period.";

        public CreateCommand(
            WeeklyService weeklyService,
            Configuration configuration)
            : base(weeklyService, configuration, CommandName, CommandDescription)
        {
            var workLogOption = new Option<string>(
                new string[] { "--workLog", "-w" },
                description: "The file name of the work log to create. Use 'p' for previous period."
            );

            var startDateOption = new Option<DateTime?>(
                new string[] { "--startDate", "-s" },
                description: "The start date of the time period to create the work log for. If not provided, the start date of the current period will be used."
            );

            var endDateOption = new Option<DateTime?>(
                new string[] { "--endDate", "-e" },
                description: "The end date of the time period to create the work log for. If not provided, the end date will be calculated using the DaysPerFile setting."
            );

            var templateOption = new Option<string>(
                new string[] { "--template", "-t" },
                description: "The template to use for the new work log. If not provided, the default template will be used."
            );

            var forceOption = new Option<bool>(
                new string[] { "--force", "-f" },
                description: "If a work log already exists with the same name, overwrite it.");

            AddOption(workLogOption);
            AddOption(startDateOption);
            AddOption(endDateOption);
            AddOption(templateOption);
            AddOption(forceOption);

            this.SetHandler(Create, workLogOption, startDateOption, endDateOption, templateOption, forceOption);

        }

        public async Task Create(string workLogName, DateTime? startDate, DateTime? endDate, string template, bool force)
        {
            try
            {
                WorkLogPeriod period = null;

                if (startDate.HasValue)
                {
                    if (endDate.HasValue)
                    {
                        period = new WorkLogPeriod(startDate.Value, endDate.Value);
                    }
                    else
                    {
                        period = new WorkLogPeriod(startDate.Value, _configuration.DaysPerFile);
                    }
                }
                else
                {
                    period = WorkLogPeriod.GetCurrentPeriod(_configuration.WeekBeginsOn[0], _configuration.DaysPerFile);
                }

                if (string.IsNullOrEmpty(workLogName))
                {
                    workLogName = period.StartDate.ToString("yyyyMMdd");
                }

                if (string.IsNullOrEmpty(template))
                {
                    template = _configuration.DefaultTemplateName;
                }

                if (_weeklyService.WorkLogExists(workLogName))
                {
                    if (!force)
                    {
                        throw new CreateConflictException(workLogName);
                    }
                    else
                    {
                        _weeklyService.DeleteWorkLog(workLogName);
                    }
                }

                var workLog = await _weeklyService.CreateWorkLogAsync(workLogName, period, template);

                WriteStatus($"Created new work log [olive]{workLogName}[/]");
                WriteStatus($"Period: {workLog.Period.ToLongString()}");
            }
            catch (ProgramException ex)
            {
                WriteProgramExceptionAndExit(ex);
            }
        }
    }
}
