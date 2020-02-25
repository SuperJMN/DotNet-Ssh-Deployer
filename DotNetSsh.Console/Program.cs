using System;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using DotNetSsh.UserSecrets;
using Grace.DependencyInjection;
using Renci.SshNet;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DotNetSsh.App
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var container = CreateContainer();

            var builder = new CommandLineBuilder()
                .UseDefaults()
                .UseExceptionHandler((exception, context) =>
                {
                    Log.Error(exception, "An error has occurred: {Error}", exception?.InnerException?.Message ?? exception?.Message);
                })
                .Configure(container)
                .Build();

            SetupLogging(builder.Parse(args).ValueForOption<bool>("--verbose"));

            await builder.InvokeAsync(args);
        }

        private static DependencyInjectionContainer CreateContainer()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(c =>
            {
                c.ExcludeTypeFromAutoRegistration("DotNetSsh.*");
                c.Export<DeploymentProfileRepository>().As<IDeploymentProfileRepository>();
                c.Export<FileSystem>().As<IFileSystem>();
                c.Export<LaunchSettingsProfileRepository>().As<ILaunchSettingsProfileRepository>();
                c.Export<UserSecretsManager>().As<IUserSecretsManager>();
                c.Export<Deployer>().As<IDeployer>();
                c.Export<FullDeployer>().As<IFullDeployer>();
                c.Export<ProfileWizard>().As<IProfileWizard>();
                c.Export<ProjectPublisher>().As<IProjectPublisher>();
                c.Export<FullDeployer>().As<IFullDeployer>();
                c.Export<SecureSession>().As<ISecureSession>();
                c.ExportFactory<string, ProjectMetadata>(ProjectMetadata.FromPath);
                c.Export<DeploymentUnit>();
                c.Export<ProfileCreationUnit>();
                c.ExportFactory<Func<string, IUserSecretsManager>, Deployment, ConnectionInfo>(InputsConverter
                    .ToConnectionInfo).As<ConnectionInfo>();
                c.ExportFactory<Func<string, IDeploymentProfileRepository>, DeploymentOptions, Deployment>(
                    InputsConverter.ToDeployment);
            });

            return container;
        }


        private static void SetupLogging(bool verbose)
        {
            var loggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .CreateLogger();

            loggingLevelSwitch.MinimumLevel = verbose ? LogEventLevel.Verbose : LogEventLevel.Information;
        }
    }
}