using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetSsh.UserSecrets;
using Serilog;
using Credentials = DotNetSsh.UserSecrets.Credentials;

namespace DotNetSsh
{
    public class ProfileCreationUnit
    {
        private readonly Func<string, IDeploymentProfileRepository> depProfileRepoFactory;
        private readonly Func<string, ILaunchSettingsProfileRepository> launchSettingsProfileRepoFactory;
        private readonly Func<string, ProjectMetadata> metadataFactory;
        private readonly ProfileCreationOptions options;
        private readonly Func<string, IUserSecretsManager> userSecretsManagerFactory;
        private readonly IProfileWizard wizard;

        public ProfileCreationUnit(ProfileCreationOptions options, IProfileWizard wizard,
            Func<string, IDeploymentProfileRepository> depProfileRepoFactory,
            Func<string, ProjectMetadata> metadataFactory,
            Func<string, ILaunchSettingsProfileRepository> launchSettingsProfileRepoFactory,
            Func<string, IUserSecretsManager> userSecretsManagerFactory)
        {
            this.wizard = wizard;
            this.options = options;
            this.depProfileRepoFactory = depProfileRepoFactory;
            this.metadataFactory = metadataFactory;
            this.launchSettingsProfileRepoFactory = launchSettingsProfileRepoFactory;
            this.userSecretsManagerFactory = userSecretsManagerFactory;
        }

        public Task Create()
        {
            ValidateAuth(options.AuthType, options.Auth);

            ConfigureAuthMethod();
            AddOrUpdateProfile();
            AddOrUpdateLaunchSettings();

            return Task.CompletedTask;
        }

        private static void ValidateAuth(AuthType authType, string auth)
        {
            var sample = Sample(authType);

            if (auth == null)
            {
                throw new ArgumentException($"Auth should have a value in the form {sample}");
            }

            var split = auth.Split(":", 2);
            if (split.Length < 2)
            {
                throw new ArgumentException($"The auth string '{auth}' isn't valid. It should be in the form {sample}");
            }
        }

        private static string Sample(AuthType authType)
        {
            switch (authType)
            {
                case AuthType.Classic:
                case AuthType.UserSecrets:

                    return "username:password";
                case AuthType.PrivateKeyFile:
                    return "username:private_key_file_path, e.g. user:\"C:\\Keys\\MyKey.key\"";
                default:
                    throw new ArgumentOutOfRangeException(nameof(authType), authType, null);
            }
        }

        private void ConfigureAuthMethod()
        {
            switch (options.AuthType)
            {
                case AuthType.Classic:
                    ShowClassicAuthWarning();
                    break;
                case AuthType.PrivateKeyFile:
                    break;
                case AuthType.UserSecrets:
                    var split = options.Auth.Split(":");
                    var username = split[0];
                    var password = split[1];

                    var secretsManager = userSecretsManagerFactory(options.Project);
                    var dict = secretsManager.FromSecrets().ToDictionary(x => x.Name, profile => profile.Credentials);
                    if (!dict.Any())
                    {
                        secretsManager.Init();
                    }

                    dict[options.Profile] = new Credentials
                    {
                        Username = username,
                        Password = password
                    };

                    secretsManager.ToSecrets(dict.Select(x => new Profile {Name = x.Key, Credentials = x.Value})
                        .ToList());

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowClassicAuthWarning()
        {
            Log.Warning(Resources.ClassicAuthWarning);
        }

        private void AddOrUpdateLaunchSettings()
        {
            var projectFile = options.Project;
            var auth = options.AuthType == AuthType.Classic || options.AuthType == AuthType.PrivateKeyFile
                ? $"--auth {options.Auth}"
                : "";

            var segments = new[]
            {
                $@"deploy {options.Profile}", "--configuration $(ConfigurationName)", $"--auth-type {options.AuthType}",
                auth
            };

            var args = string.Join(" ", segments);

            var launchSettings = new LaunchSettings.MyNamespace.Profile
            {
                CommandName = "Executable",
                ExecutablePath = "dotnet-ssh",
                WorkingDirectory = Path.GetDirectoryName(projectFile),
                CommandLineArgs = args
            };

            var launchSettingsRepo = launchSettingsProfileRepoFactory(projectFile);
            launchSettingsRepo.AddOrUpdate(options.Profile, launchSettings);
            Log.Information("Operation completed");
        }

        private void AddOrUpdateProfile()
        {
            var projectFile = options.Project;

            var repo = depProfileRepoFactory(projectFile);
            var existing = repo.Get(options.Profile);

            var username = GetUsername(options);

            var profile = wizard.Configure(options.Profile, metadataFactory(projectFile), username, existing);
            repo.AddOrUpdate(profile);
        }

        private static string GetUsername(ProfileCreationOptions profileCreationOptions)
        {
            switch (profileCreationOptions.AuthType)
            {
                case AuthType.Classic:
                case AuthType.PrivateKeyFile:
                    return profileCreationOptions.Auth.Split(":").First();
                case AuthType.UserSecrets:
                    return profileCreationOptions.Auth.Split(":").First();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}