using System;
using System.Linq;
using DotNetSsh.UserSecrets;
using Renci.SshNet;

namespace DotNetSsh.App
{
    internal static class InputsConverter
    {
        public static ConnectionInfo ToConnectionInfo(Func<string, IUserSecretsManager> userSecretsManagerFactory, Deployment options)
        {
            ValidateAuth(options.AuthType, options.Auth);

            switch (options.AuthType)
            {
                case AuthType.Classic:
                    return FromClassicAuth(options.Auth, options.Settings.Host);
                case AuthType.PrivateKeyFile:
                    return FromPrivateKeyFile(options);
                case AuthType.UserSecrets:
                    return FromUserSecrets(userSecretsManagerFactory, options);
            }

            throw new ArgumentOutOfRangeException($"Authorization for authorization type '{options.AuthType}' cannot be parsed. It should be a string in the form of {Sample(options.AuthType)}");
        }

        public static CredentialsManager ToCredentialManager(Func<string, IUserSecretsManager> userSecretsManagerFactory, Deployment options)
        {
            ValidateAuth(options.AuthType, options.Auth);

            switch (options.AuthType)
            {
                case AuthType.Classic:
                    var split = options.Auth.Split(":");
                    var username = split[0];
                    var password = split[1];
                    return new CredentialsManager()
                    {
                        UserName = username,
                        Password = password
                    };
                case AuthType.PrivateKeyFile:
                    return new CredentialsManager();
                case AuthType.UserSecrets:
                    var secrets = UserSecretsUtils.FromSecrets(userSecretsManagerFactory(options.ProjectPath));
                    var secret = secrets.First(profile => string.Equals(options.Profile.Name, profile.Name));
                    return new CredentialsManager()
                    {
                        UserName = secret.Credentials.Username,
                        Password = secret.Credentials.Password
                    };
            }

            throw new ArgumentOutOfRangeException($"Authorization for authorization type '{options.AuthType}' cannot be parsed. It should be a string in the form of {Sample(options.AuthType)}");
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

        private static ConnectionInfo FromUserSecrets(Func<string, IUserSecretsManager> userSecretsManagerFactory, Deployment options)
        {
            var secrets = UserSecretsUtils.FromSecrets(userSecretsManagerFactory(options.ProjectPath));
            var secret = secrets.First(profile => string.Equals(options.Profile.Name, profile.Name));
            var connectionInfo = new ConnectionInfo(options.Settings.Host, secret.Credentials.Username,
                new PasswordAuthenticationMethod(secret.Credentials.Username, secret.Credentials.Password));
            return connectionInfo;
        }

        private static ConnectionInfo FromPrivateKeyFile(Deployment options)
        {
            var split = options.Auth.Split(":");
            var username = split[0];
            var filePath = split[1];
            var key = new PrivateKeyAuthenticationMethod(username, new PrivateKeyFile(filePath));
            var connectionInfo = new ConnectionInfo(options.Settings.Host, username, key);
            return connectionInfo;
        }

        private static ConnectionInfo FromClassicAuth(string optionsAuth, string settingsHost)
        {
            var split = optionsAuth.Split(":");
            var username = split[0];
            var password = split[1];
            var connectionInfo = new ConnectionInfo(settingsHost, username, new PasswordAuthenticationMethod(username, password));
            return connectionInfo;
        }

        public static Deployment ToDeployment(Func<string, IDeploymentProfileRepository> repoFactory, DeploymentOptions deploymentOptions)
        {
            var profile = repoFactory(deploymentOptions.Project).Get(deploymentOptions.Profile);
            var deployment = new Deployment
            {
                Profile = profile,
                Settings = profile.Settings,
                ProjectPath = deploymentOptions.Project,
                BuildConfiguration = deploymentOptions.Configuration,
                CleanDeploymentDestination = deploymentOptions.CleanDeploymentDestination,
                AuthType = deploymentOptions.AuthType,
                Auth = deploymentOptions.Auth,
            };

            return deployment;
        }

        private static void ValidateAuth(AuthType authType, string auth)
        {
            switch (authType)
            {
                case AuthType.Classic:
                case AuthType.PrivateKeyFile:
                
                    var split = auth.Split(":", 2);
                    if (split.Length < 2)
                    {
                        throw new ArgumentException($"The auth string '{auth}' isn't valid");
                    }

                    break;

                case AuthType.UserSecrets:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}