using System.CommandLine;
using Weekly.Exceptions;
using Weekly.Services;

namespace Weekly.Commands
{
    public class PushCommand : BaseCommand
    {
        private const string CommandName = "push";
        private const string CommandDescription = "Push the work log to the time sheet service for approval.";


        public PushCommand(
            WeeklyService weeklyService,
            Configuration configuration)
            : base(weeklyService, configuration, CommandName, CommandDescription)
        {

            var workLogOption = new Option<string>(
                new string[] { "--workLog", "-w" },
                description: "The file name of the work log to push. Use 'p' for previous period. If not provided, the current work log will be used."
            );

            var forceOption = new Option<bool>(
                new string[] { "--force", "-f" },
                description: "If time entries already exist on the server, overwrite them.");

            AddAlias("p");

            AddOption(workLogOption);
            AddOption(forceOption);

            this.SetHandler(Push, workLogOption, forceOption);
        }

        public async Task Push(string workLogName, bool force)
        {
            try
            {
                var workLog = await _weeklyService.GetWorkLogAsync(workLogName);

                if (await _weeklyService.PeriodHasTimeEntries(workLog.Period))
                {
                    if (!force)
                    {
                        throw new PushConflictException();
                    }
                    else
                    {
                        int deleteSuccess = 0;
                        int deleteFailure = 0;

                        WriteStatus($"Deleting existing time entries: [olive]{workLog.Period}[/]");
                        await foreach (var result in _weeklyService.DeleteTimeEntriesForPeriod(workLog.Period))
                        {
                            WriteStatus(result.ToMarkupString());

                            if (result.IsSuccessful)
                            {
                                deleteSuccess++;
                            }
                            else
                            {
                                deleteFailure++;
                            }
                        }

                        WriteStatus($"Deleted [olive]{deleteSuccess}[/] entries successfully, [olive]{deleteFailure}[/] failed.");

                        if (deleteFailure > 0)
                        {
                            throw new PushConflictException();
                        }
                    }
                }

                WriteStatus($"Pushing worklog: [olive]{workLog.Period}[/]");

                int success = 0;
                int failure = 0;

                await foreach (var result in _weeklyService.PushWorkLogAsync(workLog))
                {
                    WriteStatus(result.ToMarkupString());

                    if (result.IsSuccessful)
                        success++;
                    else
                        failure++;
                }

                WriteStatus($"Completed. [olive]{success}[/] entries pushed successfully, [olive]{failure}[/] failed.");

            }
            catch (ProgramException pe)
            {
                WriteProgramExceptionAndExit(pe);
            }
        }
    }
}
