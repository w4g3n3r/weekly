using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using Weekly.Exceptions;
using Weekly.Infrastructure;
using Weekly.Services;

namespace Weekly.Commands
{
    public class InitializeCommand : BaseCommand
    {
        private const string CommandName = "init";
        private const string CommandDescription = "Initialize the time keeping directory.";

        public InitializeCommand(
            WeeklyService weeklyService,
            Configuration configuration) 
            : base(weeklyService, configuration, CommandName, CommandDescription)
        {
            var directoryOption = new Option<string>(
                new string[] { "--directory", "-d" },
                description: "The directory in which to store the time keeping files."
            );
            AddAlias("i");
            AddOption(directoryOption);

            this.SetHandler(Initialize, directoryOption);
        }

        public void Initialize(string directory)
        {
            try
            {
                _weeklyService.InitializeWorkLogDirectory(directory);

                var templateDirectory = Path.Combine(_configuration.WorkLogDirectory, _configuration.TemplateDirectoryName);

                WriteStatus("Initialized work log directory:");
                WriteStatus($" Work Log Directory: [olive]{_configuration.WorkLogDirectory}[/]");
                WriteStatus($" Template Directory: [olive]{templateDirectory}[/]");
                WriteStatus($" Configuration File: [olive]{Configuration.Path}[/]");
            }
            catch (ProgramException pe)
            {
                WriteProgramExceptionAndExit(pe);
            }
        }
    }
}
