using Renci.SshNet;
using Serilog;

namespace DotNetSsh
{
    public class SshDeployer
    {
        public void Deploy(string source, DeploymentOptions options)
        {
            var sftp = new SftpClient(options.Host, options.Credentials.User, options.Credentials.Password);
            var ssh = new SshClient(options.Host, options.Credentials.User, options.Credentials.Password);

            using (var clients = new Clients(sftp, ssh))
            {
                clients.Connect();

                SyncFiles(source, options, clients);
                GiveExecutablePermission(options, clients);
                RunIfSelected(options, clients.SshClient);
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

        private static void SyncFiles(string source, DeploymentOptions options,
            Clients clients)
        {
            Log.Information("Deploying files...");

            if (clients.SftpClient.Exists(options.DestinationPath))
            {
                Log.Verbose("The destination folder already exists. We are going to delete it.");
                clients.SshClient.DeleteExisting(options.DestinationPath);
            }

            clients.SshClient.MirrorDirTree(options.DestinationPath, source);
            clients.SftpClient.Copy(source, options.DestinationPath);
        }
    }
}