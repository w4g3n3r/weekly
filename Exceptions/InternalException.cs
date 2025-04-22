namespace Weekly.Exceptions
{
    internal class InternalException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Internal;

        public InternalException(Exception innerException)
            : base(Properties.Resources.InternalErrorMessage, innerException) { }

        public override string GetHelpMessage()
        {
            return string.Format(Properties.Resources.InternalHelpMessage, Configuration.Path);
        }
    }
}
