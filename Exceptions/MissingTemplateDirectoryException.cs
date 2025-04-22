namespace Weekly.Exceptions
{
    public class MissingTemplateDirectoryException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Configuration;
        public override string Message => Properties.Resources.MissingTemplateDirectoryErrorMessage;
        public MissingTemplateDirectoryException()
        {
        }

        public MissingTemplateDirectoryException(Exception? innerException) : base(null, innerException)
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
