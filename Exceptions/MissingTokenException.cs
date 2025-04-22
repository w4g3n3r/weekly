namespace Weekly.Exceptions
{
    internal class MissingTokenException : ProgramException
    {

        public MissingTokenException(string token)
            : base(string.Format(Properties.Resources.MissingTokenErrorMessage, token))
        {
        }

        public override ExitCode ExitCode => ExitCode.Configuration;

        public override string GetHelpMessage()
        {
            return Properties.Resources.MissingTokenHelpMessage;

        }
    }
}
