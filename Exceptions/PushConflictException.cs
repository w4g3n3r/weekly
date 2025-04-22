namespace Weekly.Exceptions
{
    internal class PushConflictException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Argument;

        public PushConflictException() : base(Properties.Resources.PushConflictErrorMessage) { }

        public override string GetHelpMessage()
        {
            return Properties.Resources.PushConflictHelpMessage;
        }
    }
}
