using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Weekly.Commands;
using Weekly.Infrastructure;
using Weekly.Serializers;
using Weekly.Services;

namespace Weekly
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var rootCommand = serviceProvider.GetRequiredService<RootCommand>();

            rootCommand.Invoke(args);

            var cache = serviceProvider.GetRequiredService<CacheManager>();
            cache.Persist();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.FileExists() ? Configuration.Load() : Configuration.Create();
            services.AddSingleton(config);

            // Register configuration
            services.AddSingleton(config);
            services.AddSingleton<FileManager>();
            services.AddSingleton<TokenManager>();
            services.AddSingleton<CacheManager>();
            services.AddSingleton<ApiManager>();

            // Register commands
            services.AddSingleton<InitializeCommand>();
            services.AddSingleton<StatusCommand>();
            services.AddSingleton<AddCommand>();
            services.AddSingleton<TokenCommand>();
            services.AddSingleton<PushCommand>();
            services.AddSingleton<CreateCommand>();

            // Register serializers
            services.AddSingleton<IWorkLogSerializer, DefaultWorkLogSerializer>();

            // Register service
            services.AddSingleton<WeeklyService>();

            // Register http clients
            services.AddHttpClient("jira",
                client =>
                {
                    client.BaseAddress = new Uri("https://rollick.atlassian.net/rest/api/3/");
                });
            services.AddHttpClient("tempo",
                client =>
                {
                    client.BaseAddress = new Uri("https://api.tempo.io/4/");
                });

            // Register the root command
            services.AddSingleton<RootCommand>(provider =>
            {
                var rootCommand = new RootCommand
                {
                        provider.GetRequiredService<InitializeCommand>(),
                        provider.GetRequiredService<StatusCommand>(),
                        provider.GetRequiredService<AddCommand>(),
                        provider.GetRequiredService<TokenCommand>(),
                        provider.GetRequiredService<PushCommand>(),
                        provider.GetRequiredService<CreateCommand>()
                };
                rootCommand.Description = "Weekly, an awesome little time keeping app.";
                return rootCommand;
            });
        }
    }
}
