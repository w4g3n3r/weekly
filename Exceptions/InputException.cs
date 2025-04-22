namespace Weekly.Exceptions
{
    internal class InputException : ProgramException
    {

        public InputException(string? message) : base(message)
        {
        }

        public override ExitCode ExitCode => ExitCode.Argument;

        public override string GetHelpMessage()
        {
            return Properties.Resources.InputHelpMessage;
        }
    }
}
