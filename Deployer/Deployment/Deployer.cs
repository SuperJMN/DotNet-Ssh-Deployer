using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace NetCoreSsh.Deployment
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
            var projectPath = Project.GetProjectFilePath();

            if (projectPath == null)
            {
                throw new InvalidOperationException("Cannot find a project file to deploy");
            }

            var optionFromDefaults = GetDefaultOptions(projectPath);
            var mergedOptions = MergeOptions(optionFromDefaults, optionsFromFile, optionsFromCommandLine);

            return new DeploymentOptions
            {
                Project = projectPath,
                DestinationPath = mergedOptions.Destination,
                Host = mergedOptions.Host,
                Framework = mergedOptions.Framework,
                Display = mergedOptions.Display,
                Runtime = mergedOptions.Runtime,
                RunAfterDeployment = mergedOptions.RunAfterDeployment,
                PublishFolder = Project.GetPublishFolder(projectPath, mergedOptions.Runtime),
                AssemblyName = Project.GetAssemblyName(projectPath),
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
                ProjectName = b.ProjectName ?? a.ProjectName,
                Host = b.Host ?? a.Host,
                Destination = b.Destination ?? a.Destination,
                Password = b.Password ?? a.Password,
                UserName = b.UserName ?? a.UserName,
                Framework = b.Framework ?? a.Framework,
                Display = b.Display ?? a.Display,
                Runtime = b.Runtime ?? a.Runtime,
                RunAfterDeployment = b.RunAfterDeployment || a.RunAfterDeployment,
            };
        }

        public static DeploymentUserOptions GetDefaultOptions(string projectPath, CreateTemplateOptions templateOptions = null)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectPath);

            var userName = templateOptions?.UserName ?? "{username}";
            var host = templateOptions?.Host ?? "{host}";
            var runtime = templateOptions != null ? GetRuntime(templateOptions.Target) : null;
            var password = templateOptions?.Password ?? "{password}";

            return new DeploymentUserOptions
            {
                Destination = $"/home/{userName}/DotNetApps" + "/" + projectName,
                Host = host,
                Password = password,
                UserName = userName,
                Framework = Project.GetFramework(projectPath),
                ProjectName = projectName,
                Runtime = runtime,
                Display = ":0.0",
            };
        }

        private static string GetRuntime(TargetDevice templateOptions)
        {
            switch (templateOptions)
            {
                case TargetDevice.Raspbian:
                    return "linux-arm";
                case TargetDevice.GenericLinux64:
                    return "linux-x64";
                default:
                    throw new ArgumentOutOfRangeException(nameof(templateOptions), templateOptions, null);
            }
        }

        private static DeploymentUserOptions LoadFromFile(string sshDeploymentJson)
        {
            try
            {
                if (File.Exists(sshDeploymentJson))
                {
                    return JsonConvert.DeserializeObject<DeploymentUserOptions>(File.ReadAllText(sshDeploymentJson));
                }

                return new DeploymentUserOptions();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Couldn't load options file");
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

            Log.Information("Finished!");
        }
    }
}