namespace Weekly.Exceptions
{
    internal class ApiUnauthorizedException : ProgramException
    {
        private readonly string _apiName;
        public override ExitCode ExitCode => ExitCode.Configuration;
        public ApiUnauthorizedException(string apiName, Exception? innerException)
            : base(string.Format(Properties.Resources.ApiUnauthorizedErrorMessage, apiName), innerException)
        {
            _apiName = apiName;
        }

        public override string GetHelpMessage()
        {
            return string.Format(Properties.Resources.ApiUnauthorizedHelpMessage, _apiName);
        }
    }
}
