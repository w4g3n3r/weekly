namespace Weekly.Exceptions
{
    public class WorkLogNotFoundException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Argument;

        public WorkLogNotFoundException() : base(Properties.Resources.WorkLogNotFoundErrorMessage) { }

        public WorkLogNotFoundException(string? message) : base(message)
        {
        }

        public WorkLogNotFoundException(Exception? innerException)
            : base(Properties.Resources.WorkLogNotFoundErrorMessage, innerException) { }

        public WorkLogNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public override string GetHelpMessage()
        {
            return Properties.Resources.WorkLogNotFoundHelpMessage;
        }
    }
}
