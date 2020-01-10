using System;
using System.IO;
using System.Threading.Tasks;
using Renci.SshNet;
using Serilog;

namespace DotNetSsh
{
    public class SshDeployer
    {
        public async Task Deploy(DirectoryInfo source, DeploymentOptions options, bool cleanTargetFolder)
        {
            try
            {
                Log.Information("Starting deployment...");

                var sftp = new SftpClient(options.Host, options.Credentials.User, options.Credentials.Password);
                var ssh = new SshClient(options.Host, options.Credentials.User, options.Credentials.Password);

                using var clients = new Clients(sftp, ssh);
                clients.Connect();

                await SyncFiles(source, options, clients, cleanTargetFolder);
                GiveExecutablePermission(options, clients);
                RunIfSelected(options, clients.SshClient);
            }
            catch (Exception e)
            {
                throw new DeploymentException("Deployment failed", e);
            }
        }

        private static void RunIfSelected(DeploymentOptions options, SshClient ssh)
        {
            if (!options.RunAfterDeployment)
            {
                return;
            }

            Log.Information($"Running application on display {options.Display}");
            var commandPath = GetExecutableName(options);
            Log.Information("Application is running!");
            Log.Information("Waiting for the application to be closed...");
            Log.Warning("(this command will wait for the application to finish. Close it before trying to deploy again)");
            ssh.RunCommand($"DISPLAY={options.Display} nohup {commandPath}");
        }

        private static void GiveExecutablePermission(DeploymentOptions options, Clients clients)
        {
            var executable = GetExecutableName(options);
            clients.SshClient.RunCommand($"chmod +x {executable}");
        }

        private static string GetExecutableName(DeploymentOptions options)
        {
            return options.DestinationPath + "/" + options.AssemblyName;
        }

        private static Task SyncFiles(DirectoryInfo source, DeploymentOptions options,
            Clients clients, bool cleanTargetFolder)
        {
            Log.Information("Deploying files...");

            PrepareTargetFolder(options, clients, cleanTargetFolder);
            var synchronizer = new SshFolderSynchronizer(clients.SftpClient);
            return synchronizer.Sync(source, options.DestinationPath);
        }

        private static void PrepareTargetFolder(DeploymentOptions options, Clients clients, bool cleanTargetFolder)
        {
            if (cleanTargetFolder && clients.SftpClient.Exists(options.DestinationPath))
            {
                Log.Verbose("The destination folder already exists. We are going to delete it.");
                clients.SshClient.DeleteExisting(options.DestinationPath);
            }
        }
    }
}