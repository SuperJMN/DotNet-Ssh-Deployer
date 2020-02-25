using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Grace.DependencyInjection;

namespace DotNetSsh.App
{
    internal static class ConfigureMixin
    {
        private static string GetDefaultProject()
        {
            try
            {
                var firstOrDefault = Directory
                    .GetFiles(Environment.CurrentDirectory, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (firstOrDefault == null)
                {
                    throw new ArgumentException("Cannot find any project in the current directory");
                }

                return firstOrDefault;
            }
            catch (Exception e)
            {
                throw new IOException("An error has occurred while looking for the project file in the current directory.", e);
            }
        }

        public static CommandLineBuilder Configure(this CommandLineBuilder builder, ILocatorService container)
        {
            var project = new Option<string>("--project", GetDefaultProject);
            var authType = new Option<AuthType>("--auth-type", "Authentication") {Required = true};
            var auth = new Option<string>("--auth", "Authentication string");
            var profile = new Argument<string>("profile");
            var verbose = new Option<bool>("--verbose", () => false);

            var configureCommand = new Command("configure")
            {
                profile,
                project,
                authType,
                auth,
                verbose
            };

            configureCommand.Handler = CommandHandler.Create<ProfileCreationOptions>(options =>
            {
                var creator = container.Locate<ProfileCreationUnit>(options);
                return creator.Create();
            });

            var deployCommand = new Command("deploy")
            {
                profile,
                project,
                authType,
                auth,
                new Option<string>("--configuration", () => "Debug", "Build configuration"),
                verbose
            };

            deployCommand.Handler = CommandHandler.Create<DeploymentOptions>(options =>
            {
                var creator = container.Locate<DeploymentUnit>(options);
                return creator.Deploy();
            });

            builder.AddCommand(configureCommand);
            builder.AddCommand(deployCommand);
            return builder;
        }
    }
}