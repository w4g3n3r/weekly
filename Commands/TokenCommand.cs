using System.CommandLine;
using Weekly.Exceptions;
using Weekly.Services;

namespace Weekly.Commands
{
    public class TokenCommand : BaseCommand
    {
        private const string CommandName = "token";
        private const string CommandDescription = "Register API tokens used to connect to the time keeping APIs.";
        public TokenCommand(
            WeeklyService weeklyService,
            Configuration configuration)
            : base(weeklyService, configuration, CommandName, CommandDescription)
        {
            var tokenNameArgument = new Argument<string>("token name", "The name for which this token will be referenced.");
            var userNameArgument = new Argument<string>("user name", "The user name that accompanies the token.");
            var secretArgument = new Argument<string>("secret", "The token.");

            var addCommand = new Command("add", "Add or update an API token.");
            addCommand.AddArgument(tokenNameArgument);
            addCommand.AddArgument(userNameArgument);
            addCommand.AddArgument(secretArgument);
            addCommand.SetHandler(Add, tokenNameArgument, userNameArgument, secretArgument);

            var removeCommand = new Command("rm", "Removes an existing token.");
            removeCommand.AddArgument(tokenNameArgument);
            removeCommand.SetHandler(Remove, tokenNameArgument);

            var clearCommand = new Command("clr", "Clears all the stored tokens.");
            clearCommand.SetHandler(Clear);

            this.AddCommand(addCommand);
            this.AddCommand(removeCommand);
            this.AddCommand(clearCommand);
        }

        public void Add(string tokenName, string userName, string secret)
        {
            try
            {
                _weeklyService.AddToken(tokenName, userName, secret);

                WriteStatus($"Added token [olive]{tokenName}[/].");
            }
            catch (ProgramException pe)
            {
                WriteProgramExceptionAndExit(pe);
            }
        }

        public void Remove(string tokenName)
        {
            try
            {
                _weeklyService.RemoveToken(tokenName);

                WriteInformation($"Removed token [olive]{tokenName}[/].");
            }
            catch (ProgramException pe)
            {
                WriteProgramExceptionAndExit(pe);
            }


        }

        public void Clear()
        {
            try
            {
                _weeklyService.ClearTokens();

                WriteInformation($"All tokens stored by [teal]Weekly[/] have been removed.");
            }
            catch (ProgramException pe)
            {
                WriteProgramExceptionAndExit(pe);
            }
        }
    }
}
