namespace Weekly.Exceptions
{
    internal class ApiException : ProgramException
    {
        public ApiException(string apiName) : base(apiName) { }
        public ApiException(string apiName, Exception ex)
            : base(string.Format(Properties.Resources.ApiErrorMessage, apiName), ex)
        {
        }

        public override ExitCode ExitCode => ExitCode.Internal;

        public override string GetHelpMessage()
        {
            return string.Format(Properties.Resources.ApiHelpMessage, Configuration.Path);
        }
    }
}
