namespace Weekly.Exceptions
{
    internal class CreateConflictException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Argument;

        public CreateConflictException(string? workLogName)
            : base(string.Format(Properties.Resources.CreateConflictErrorMessage, workLogName))
        {
        }

        public override string GetHelpMessage()
        {
            return Properties.Resources.CreateConflictHelpMessage;
        }
    }
}
