using System.Text;

namespace Weekly.Exceptions
{
    public enum ExitCode
    {
        Success = 0,
        Configuration = 1,
        Argument = 2,
        Internal = 3
    }
    public class ProgramException : Exception
    {
        public virtual string? HelpMessage => GetHelpMessage();
        public virtual ExitCode ExitCode => ExitCode.Internal;

        public ProgramException()
        {
        }

        public ProgramException(string? message) : base(message)
        {
        }

        public ProgramException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public virtual string GetHelpMessage()
        {
            var sb = new StringBuilder();
            sb.Append("An unexpected error has occured within the program. If this error persisits, ");
            sb.Append("you can enable the output of exceptions to the console via the config file. ");
            sb.Append("Your config file should be here: [yellow]");
            sb.Append(Configuration.Path);
            sb.Append("[/]. Look for the setting named [yellow]LogExceptionsToConsole[/] and update ");
            sb.Append("it to [yellow]true[/].");
            sb.AppendLine();
            sb.AppendLine("Help is available via [yellow]wk --help[/].");
            sb.AppendLine();

            return sb.ToString();

        }

    }
}
