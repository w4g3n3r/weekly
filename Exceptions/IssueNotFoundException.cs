namespace Weekly.Exceptions
{
    internal class IssueNotFoundException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Argument;

        public IssueNotFoundException(string issueKey, Exception? innerException)
            : base(string.Format(Properties.Resources.IssueNotFoundErrorMessage, issueKey), innerException)
        {
        }

        public override string GetHelpMessage()
        {
            return string.Format(Properties.Resources.IssueNotFoundHelpMessage, Configuration.Path);
            ;
        }
    }
}
