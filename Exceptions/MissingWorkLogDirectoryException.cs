namespace Weekly.Exceptions
{
    public class MissingWorkLogDirectoryException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Configuration;
        public override string Message => Properties.Resources.MissingWorkLogDirectoryErrorMessage;
        public MissingWorkLogDirectoryException()
        {
        }

        public MissingWorkLogDirectoryException(Exception? innerException) : base(null, innerException)
        {
        }

        public override string GetHelpMessage()
        {
            return string.Format(
                Properties.Resources.MissingWorkLogDirectoryHelpMessage,
                Configuration.Path);
        }
    }
}
