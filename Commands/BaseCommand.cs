using Spectre.Console;
using System.CommandLine;
using Weekly.Exceptions;
using Weekly.Services;

namespace Weekly.Commands
{
    public abstract class BaseCommand : Command
    {
        protected readonly WeeklyService _weeklyService;
        protected readonly Configuration _configuration;

        public BaseCommand(
            WeeklyService weeklyService,
            Configuration configuration,
            string name,
            string description)
            : base(name, description)
        {
            _weeklyService = weeklyService ?? throw new ArgumentNullException(nameof(weeklyService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected void WriteProgramExceptionAndExit(ProgramException ex)
        {
            Console.WriteLine();

            WriteError(ex.Message);

            if (!string.IsNullOrEmpty(ex.HelpMessage) && _configuration.ShowHelpMessages.Value)
            {
                var rule = new Rule(" [teal]Help[/] ");
                rule.Justification = Justify.Left;
                rule.Style = new Style(foreground: Color.Grey23);
                AnsiConsole.Write(rule);

                WriteInformation(ex.HelpMessage);
                WriteInformation(Properties.Resources.HelpInstructionMessage);
            }

            if (ex.InnerException != null && _configuration.LogExceptionsToConsole.Value)
            {
                var rule = new Rule(" [maroon]Exception[/] ");
                rule.Justification = Justify.Left;
                rule.Style = new Style(foreground: Color.Grey23);
                AnsiConsole.Write(rule);

                WriteException(ex.InnerException);
            }

            Environment.Exit((int)ex.ExitCode);
        }

        protected void WriteError(string message)
        {
            var rule = new Rule(" [maroon]Error[/] ");
            rule.Justification = Justify.Left;
            rule.Style = new Style(foreground: Color.Grey23);

            var messageText = new Markup(message);
            var paddedText = new Padder(messageText)
                .Padding(5, 1);
            AnsiConsole.Write(rule);
            AnsiConsole.Write(paddedText);
        }

        protected void WriteInformation(string message)
        {
            var messageMarkup = new Markup(message);
            var messagePadding = new Padder(messageMarkup)
                .Padding(5, 1);

            AnsiConsole.Write(messagePadding);
        }

        protected void WriteStatus(string status)
        {
            var messageMarkup = new Markup(status);
            var messagePadding = new Padder(messageMarkup)
                .Padding(5, 0);

            AnsiConsole.Write(messagePadding);
        }

        protected void WriteException(Exception exception)
        {
            var exceptionPadding = new Padder(exception.GetRenderable(ExceptionFormats.ShortenEverything))
                .Padding(5, 1);

            AnsiConsole.Write(exceptionPadding);
        }
    }
}
