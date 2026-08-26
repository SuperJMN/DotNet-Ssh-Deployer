using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using Grace.DependencyInjection;
using Serilog;

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

        public static RootCommand Configure(this ILocatorService container)
        {
            var configureProject = new Option<string>("--project", Array.Empty<string>())
            {
                DefaultValueFactory = _ => GetDefaultProject()
            };
            var configureAuthType = new Option<AuthType>("--auth-type", Array.Empty<string>())
            {
                Description = "Authentication",
                Required = true
            };
            var configureAuth = new Option<string>("--auth", Array.Empty<string>())
            {
                Description = "Authentication string"
            };
            var configureProfile = new Argument<string>("profile");
            var configureVerbose = new Option<bool>("--verbose", Array.Empty<string>());

            var configureCommand = new Command("configure", string.Empty)
            {
                configureProfile,
                configureProject,
                configureAuthType,
                configureAuth,
                configureVerbose
            };

            configureCommand.SetAction(async parseResult =>
            {
                var options = new ProfileCreationOptions
                {
                    Profile = parseResult.GetValue(configureProfile),
                    Project = parseResult.GetValue(configureProject),
                    AuthType = parseResult.GetValue(configureAuthType),
                    Auth = parseResult.GetValue(configureAuth),
                    Verbose = parseResult.GetValue(configureVerbose)
                };

                var creator = container.Locate<ProfileCreationUnit>(options);
                await creator.Create();
            });

            var deployProject = new Option<string>("--project", Array.Empty<string>())
            {
                DefaultValueFactory = _ => GetDefaultProject()
            };
            var deployAuthType = new Option<AuthType>("--auth-type", Array.Empty<string>())
            {
                Description = "Authentication",
                Required = true
            };
            var deployAuth = new Option<string>("--auth", Array.Empty<string>())
            {
                Description = "Authentication string"
            };
            var deployProfile = new Argument<string>("profile");
            var deployConfiguration = new Option<string>("--configuration", Array.Empty<string>())
            {
                Description = "Build configuration",
                DefaultValueFactory = _ => "Debug"
            };
            var deployVerbose = new Option<bool>("--verbose", Array.Empty<string>());

            var deployCommand = new Command("deploy", string.Empty)
            {
                deployProfile,
                deployProject,
                deployAuthType,
                deployAuth,
                deployConfiguration,
                deployVerbose
            };

            deployCommand.SetAction(async parseResult =>
            {
                var options = new DeploymentOptions
                {
                    Profile = parseResult.GetValue(deployProfile),
                    Project = parseResult.GetValue(deployProject),
                    AuthType = parseResult.GetValue(deployAuthType),
                    Auth = parseResult.GetValue(deployAuth),
                    Configuration = parseResult.GetValue(deployConfiguration),
                    Verbose = parseResult.GetValue(deployVerbose)
                };

                var deploymentRequest = container.Locate<DeploymentUnit>(options);
                var deploy = await deploymentRequest.Deploy();
                if (deploy.IsFailure)
                {
                    Log.Error($"Deployment failed {deploy.Error}");
                }
            });

            var rootCommand = new RootCommand(string.Empty);
            rootCommand.Add(configureCommand);
            rootCommand.Add(deployCommand);
            return rootCommand;
        }
    }
}
