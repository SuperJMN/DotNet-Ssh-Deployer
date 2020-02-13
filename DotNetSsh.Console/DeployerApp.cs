using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetSsh.Console.Verbs;
using DotNetSsh.LaunchSettings;
using DotNetSsh.LaunchSettings.MyNamespace;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DotNetSsh.Console
{
    public class DeployerApp
    {
        private readonly Func<string, IDeploymentProfileRepository> repoFactory;

        public DeployerApp(Func<string, IDeploymentProfileRepository> repoFactory)
        {
            this.repoFactory = repoFactory;
        }

        private const string ProfileStoreFilename = "ssh-deployment.json";

        public async Task Deploy(DeployVerbOptions verbOptions)
        {
            SetupLogging(verbOptions.Verbose);

            var projectFile = LookupProjectFile(verbOptions.ProjectFile);
            var profile = LookupProfile(projectFile, verbOptions.Name);

            var deployer = new SshDeployer();
            var publisher = new ProjectPublisher();

            var publishPath = publisher.Publish(projectFile, profile.Options.TargetDevice, profile.Options.Framework, verbOptions.Configuration);
            await deployer.Deploy(new DirectoryInfo(publishPath), profile.Options, verbOptions.CleanTarget);

            Log.Information($"Operation finished");
        }

        public void AddOrReplaceProfile(CreateVerbOptions verbOptions)
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

            var repo = repoFactory(ProfileStoreFilename);
            repo.Add(new DeploymentProfile(verbOptions.Name, ops));

            Log.Information($"Profile '{verbOptions.Name}' created successfully");
        }

        private DeploymentProfile LookupProfile(string projectFile, string profileName)
        {
            var projectDir = Path.GetDirectoryName(projectFile);

            var repo = repoFactory(Path.Combine(projectDir, ProfileStoreFilename));
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

        public Task ConfigureLaunchSettings(ConfigureVerbOptions options)
        {
            SetupLogging(options.Verbose);

            var project = LookupProjectFile(options.Project);
            Log.Information("Configuring launch settings for {Project}", project);
            var basePath = Path.GetDirectoryName(project);
            var repo = repoFactory(Path.Combine(basePath, ProfileStoreFilename));
            var profiles = repo.GetAll();
            var launchSettings = GetLaunchSettings(basePath);

            foreach (var profile in profiles)
            {
                var action = launchSettings.Profiles.ContainsKey(profile.Name) ? "Updating" : "Creating";
                Log.Verbose($"{action} {{Entry}}", profile.Name);
                launchSettings.Profiles[profile.Name] = new Profile
                {
                    CommandName = "Executable",
                    ExecutablePath = "dotnet-ssh",
                    WorkingDirectory = basePath,
                    CommandLineArgs = $@"deploy -p {profile.Name} -c $(ConfigurationName)",
                };
            }

            SaveLaunchSettings(launchSettings, basePath);

            Log.Information($"Launch settings configured. You can now deploy using any IDE that uses launchsettings.json :)");

            return Task.CompletedTask;
        }

        private void SaveLaunchSettings(LaunchSettingsRoot launchSettings, string basePath)
        {
            var path = Path.Combine(basePath, "Properties", "launchsettings.json");

            var serialized = JsonConvert.SerializeObject(launchSettings, Formatting.Indented);
            File.WriteAllText(path, serialized);
        }

        private LaunchSettingsRoot GetLaunchSettings(string basePath)
        {
            var path = Path.Combine(basePath, "Properties", "launchsettings.json");
            if (File.Exists(path))
            {
                var contents = File.ReadAllText(path);
                var root = JsonConvert.DeserializeObject<LaunchSettingsRoot>(contents);

                return root;
            }

            return new LaunchSettingsRoot()
            {
                Profiles = new Dictionary<string, Profile>()
            };
        }
    }
}