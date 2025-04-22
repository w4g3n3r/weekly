namespace Weekly.Exceptions
{
    public class WorkLogParseException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Configuration;
        public WorkLogParseException()
            : base(Properties.Resources.WorkLogParseErrorMessage) { }

        public WorkLogParseException(Exception? innerException)
            : base(Properties.Resources.WorkLogParseErrorMessage, innerException)
        {
        }

        public override string GetHelpMessage()
        {
            return Properties.Resources.WorkLogParseHelpMessage;
        }
    }
}
