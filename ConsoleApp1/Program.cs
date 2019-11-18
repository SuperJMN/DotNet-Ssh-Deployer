using System;
using System.IO;
using System.Linq;
using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DotNetSsh.Console
{
    class Program
    {
        private const string ProfileStoreFilename = "deployment.json";

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<AddVerbOptions, DeployVerbOptions>(args)
                .MapResult(
                    (AddVerbOptions o) => AddOrReplaceProfile(o), 
                    (DeployVerbOptions o) => Deploy(o), errors => 1);
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

        private static int Deploy(DeployVerbOptions verbOptions)
        {
            SetupLogging(verbOptions.Verbose);

            var projectFile = LookupProjectFile(verbOptions.ProjectFile);
            var profile = LookupProfile(projectFile, verbOptions.Name);
            
            var deployer = new SshDeployer();
            var publisher = new ProjectPublisher();
            
            var publishPath = publisher.Publish(projectFile, profile.Options.TargetDevice, profile.Options.Framework);

            deployer.Deploy(publishPath, profile.Options);

            return 0;
        }

        private static DeploymentProfile LookupProfile(string projectFile, string profileName)
        {
            var projectDir = Path.GetDirectoryName(projectFile);

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

        private static int AddOrReplaceProfile(AddVerbOptions verbOptions)
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

            return 0;
        }
    }
}
