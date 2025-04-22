namespace Weekly.Exceptions
{
    public class TemplateNotFoundException : ProgramException
    {
        public override ExitCode ExitCode => ExitCode.Argument;

        public TemplateNotFoundException() : base(Properties.Resources.WorkLogNotFoundErrorMessage) { }

        public TemplateNotFoundException(string? message) : base(message)
        {
        }

        public TemplateNotFoundException(Exception? innerException)
            : base(Properties.Resources.TemplateNotFoundErrorMessage, innerException) { }

        public TemplateNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public override string GetHelpMessage()
        {
            return Properties.Resources.TemplateNotFoundHelpMessage;
        }
    }
}
