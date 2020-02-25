using System;
using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using Serilog;

namespace DotNetSsh
{
    public class Deployer : IDeployer
    {
        private readonly Func<ISecureSession> secureSession;

        public Deployer(Func<ISecureSession> secureSession)
        {
            this.secureSession = secureSession;
        }

        public async Task Deploy(DirectoryInfo source, Deployment settings)
        {
            try
            {
                Log.Information("Starting deployment...");

                using var clients = secureSession();
                await SyncFiles(source, settings, clients);
                GiveExecutablePermission(settings.Settings, clients);
                RunIfSelected(settings, clients.Ssh);
            }
            catch (Exception e)
            {
                throw new DeploymentException("Deployment failed", e);
            }
        }

        private static void RunIfSelected(Deployment settings, SshClient ssh)
        {
            if (!settings.Settings.RunAfterDeployment)
            {
                return;
            }

            Log.Information($"Running application on display {settings.Settings.RunAfterDeployment}");
            var commandPath = GetExecutableName(settings.Settings);
            Log.Information("Application is running!");
            Log.Information("Waiting for the application to be closed...");
            Log.Warning("(this command will wait for the application to finish. Close it before trying to deploy again)");
            ssh.RunCommand($"DISPLAY={settings.Settings.Display} nohup {commandPath}");
        }

        private static void GiveExecutablePermission(CustomizableSettings settings, ISecureSession userAndPasswordSecureSession)
        {
            var executable = GetExecutableName(settings);
            userAndPasswordSecureSession.Ssh.RunCommand($"chmod +x {executable}");
        }

        private static string GetExecutableName(CustomizableSettings settings)
        {
            return settings.DestinationPath + "/" + settings.AssemblyName;
        }

        private static Task SyncFiles(DirectoryInfo source, Deployment settings,
            ISecureSession userAndPasswordSecureSession)
        {
            Log.Information("Deploying files...");

            PrepareTargetFolder(settings, userAndPasswordSecureSession);
            var synchronizer = new SshFolderSynchronizer(userAndPasswordSecureSession.Sftp);
            return synchronizer.Sync(source, settings.Settings.DestinationPath);
        }

        private static void PrepareTargetFolder(Deployment settings, ISecureSession userAndPasswordSecureSession)
        {
            if (settings.CleanDeploymentDestination && userAndPasswordSecureSession.Sftp.Exists(settings.Settings.DestinationPath))
            {
                Log.Verbose("The destination folder already exists. We are going to delete it.");
                userAndPasswordSecureSession.Ssh.DeleteExisting(settings.Settings.DestinationPath);
            }
        }
    }
}