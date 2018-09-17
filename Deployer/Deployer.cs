using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SshDeploy
{
    internal static class Deployer
    {
        public static int Deploy(DeploymentUserOptions o)
        {
            Console.WriteLine(".NET SSH/SFTP Deployment Utility for NET Core");
            Log.Information("Starting deployment...");

            try
            {
                SetupLogging(o);

                var options = GetDeplomentOptions(o);

                Log.Verbose("Using options {@Options}", options);

                Deploy(options);
            }
            catch (Exception e)
            {
                Log.Error(e, "Deployment failed");
                return 1;
            }

            return 0;
        }

        private static DeploymentOptions GetDeplomentOptions(DeploymentUserOptions o)
        {
            var optionsFromFile = LoadFromFile(Program.OptionsFilename);
            var optionsFromCommandLine = o;
            var optionFromDefaults = GetDefaultOptions("MyApp");

            var mergedOptions = MergeOptions(optionFromDefaults, optionsFromFile, optionsFromCommandLine);

            var projectFile = mergedOptions.Project ?? Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj").FirstOrDefault();

            if (projectFile == null)
            {
                throw new InvalidOperationException("Cannot find a project file to deploy");
            }

            return new DeploymentOptions
            {
                Project = projectFile,
                DestinationPath = mergedOptions.Destination,
                Host = mergedOptions.Host,
                
                Credentials = new Credentials
                {
                    Password = mergedOptions.Password,
                    User = mergedOptions.UserName
                },                
            };
        }

        private static DeploymentUserOptions MergeOptions(
            params DeploymentUserOptions[] optionFromDefaults)
        {
            var commandLineOptions = optionFromDefaults.Aggregate(InnerMerge);
            return commandLineOptions;
        }

        private static DeploymentUserOptions InnerMerge(DeploymentUserOptions a,
            DeploymentUserOptions b)
        {
            return new DeploymentUserOptions
            {
                Project = b.Project ?? a.Project,
                Host = b.Host ?? a.Host,
                Destination = b.Destination ?? a.Destination,
                Password = b.Password ?? a.Password,
                UserName = b.UserName ?? a.UserName,
            };
        }

        public static DeploymentUserOptions GetDefaultOptions(string projectName)
        {
            return new DeploymentUserOptions
            {
                Destination = "/home/pi/DotNetApps" + "/" + projectName,
                Host = "raspberrypi",
                Password = "raspberry",
                UserName = "pi"
            };
        }

        private static DeploymentUserOptions LoadFromFile(string sshDeploymentJson)
        {
            try
            {
                return JsonConvert.DeserializeObject<DeploymentUserOptions>(File.ReadAllText(sshDeploymentJson));
            }
            catch
            {
                return new DeploymentUserOptions();
            }
        }

        private static void SetupLogging(DeploymentUserOptions o)
        {
            var loggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .CreateLogger();

            loggingLevelSwitch.MinimumLevel = o.Verbose ? LogEventLevel.Verbose : LogEventLevel.Information;
        }


        private static void Deploy(DeploymentOptions options)
        {
            Log.Information("Working...");

            Program.Publish(options);
            Program.RemoteOperations(options);

            Log.Information("Deployment done!");
        }
    }
}