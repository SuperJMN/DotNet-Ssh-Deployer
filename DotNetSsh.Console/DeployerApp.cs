using System;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DotNetSsh.Console
{
    public class DeployerApp
    {
        private const string ProfileStoreFilename = "ssh-deployment.json";

        public void Deploy(DeployVerbOptions verbOptions)
        {
            SetupLogging(verbOptions.Verbose);

            var projectFile = LookupProjectFile(verbOptions.ProjectFile);
            var profile = LookupProfile(projectFile, verbOptions.Name);

            var deployer = new SshDeployer();
            var publisher = new ProjectPublisher();

            var publishPath = publisher.Publish(projectFile, profile.Options.TargetDevice, profile.Options.Framework);
            deployer.Deploy(publishPath, profile.Options);

            Log.Information($"Operation finished");
        }

        public void AddOrReplaceProfile(AddVerbOptions verbOptions)
        {
            SetupLogging(verbOptions.Verbose);
            
            DeploymentOptions ops;
            if (verbOptions.Project != null)
            {
                ops = new DeploymentOptionsBuilder()
                    .ForDevice(verbOptions.TargetDevice)
                    .FromProject(verbOptions.Project)
                    .Build();
            }
            else
            {
                ops = new DeploymentOptionsBuilder()
                    .ForDevice(verbOptions.TargetDevice)
                    .Build();
            }

            var repo = new DeploymentProfileRepository(ProfileStoreFilename);
            repo.Add(new DeploymentProfile(verbOptions.Name, ops));

            Log.Information($"Profile '{verbOptions.Name}' created successfully");
        }

        private static DeploymentProfile LookupProfile(string projectFile, string profileName)
        {
            var projectDir = Path.GetDirectoryName(projectFile);

            if (!File.Exists(ProfileStoreFilename))
            {
                throw new FileNotFoundException($"Project store file ('{ProfileStoreFilename}') doesn't exist. Please, run this tool with the 'create' verb first.");
            }

            var repo = new DeploymentProfileRepository(Path.Combine(projectDir, ProfileStoreFilename));
            var profile = repo.Get(profileName);

            if (profile == null)
            {
                throw new InvalidOperationException($"Cannot find a profile named {profileName}");
            }

            return profile;
        }

        private static string LookupProjectFile(string projectFile)
        {
            var project = projectFile ?? Directory
                              .GetFiles(Environment.CurrentDirectory, "*.csproj", SearchOption.AllDirectories)
                              .FirstOrDefault();

            if (project == null)
            {
                throw new InvalidOperationException("Cannot find a project file to deploy");
            }

            return project;
        }

        private static void SetupLogging(bool isVerbose)
        {
            var loggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .CreateLogger();

            loggingLevelSwitch.MinimumLevel = isVerbose ? LogEventLevel.Verbose : LogEventLevel.Information;
        }
    }
}