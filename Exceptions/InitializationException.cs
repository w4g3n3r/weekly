namespace Weekly.Exceptions
{
    public class InitializationException : ProgramException
    {
        public InitializationException(string workLogDirectory)
            : base(string.Format(Properties.Resources.InitializationErrorMessage, workLogDirectory)) { }

        public InitializationException(string workLogDirectory, Exception? innerException)
            : base(string.Format(Properties.Resources.InitializationErrorMessage, workLogDirectory), innerException)
        {
        }

        public override ExitCode ExitCode => ExitCode.Internal;

        public override string GetHelpMessage()
        {
            return string.Format(Properties.Resources.InitializationHelpMessage, Configuration.Path);
        }
    }
}
