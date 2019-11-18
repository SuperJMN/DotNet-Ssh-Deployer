﻿using System.IO;
using Renci.SshNet;
using Serilog;

namespace NetCoreSsh
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

                SyncFiles(options, clients);
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

        private static void SyncFiles(DeploymentOptions options,
            Clients clients)
        {
            Log.Information("Deploying files...");

            if (clients.SftpClient.Exists(options.DestinationPath))
            {
                Log.Verbose("The destination folder already exists. We are going to delete it.");
                DeleteExisting(options, clients.SshClient);
            }

            PrepareOutputDirectory(options.DestinationPath, options.DestinationPath, clients.SshClient);

            CopyOutput(options, clients.SftpClient);
        }

        private static void PrepareOutputDirectory(string destination, string origin, SshClient client)
        {
            CreateDirectory(destination, client);

            var subDirsInParent = Directory.GetDirectories(origin, "*.*", SearchOption.AllDirectories);
            foreach (var subdirInParent in subDirsInParent)
            {
                var transformToDest = subdirInParent.Replace(origin, "").Replace("\\", "/");
                var finalDir = destination + transformToDest;
                CreateDirectory(finalDir, client);
            }
        }

        private static void CreateDirectory(string destinationPath, SshClient clientsSshClient)
        {
            Log.Verbose("Creating destination directory {Directory}", destinationPath);
            clientsSshClient.RunCommand($"mkdir -p {destinationPath}");
        }

        private static void DeleteExisting(DeploymentOptions options, SshClient sshClient)
        {
            Log.Verbose("Deleting previous {Directory}", options.DestinationPath);

            sshClient.RunCommand($"rm -rf {options.DestinationPath}");
        }

        private static void CopyOutput(DeploymentOptions options, SftpClient client)
        {
            var dir = new DirectoryInfo(options.DestinationPath);
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

            int total = files.Length;
            int copied = 0;
            foreach (var file in files)
            {
                var percent = (float)copied / total;
                Log.Verbose("{Percentage:P}", percent);

                var fileDestination = file.FullName.Replace(options.DestinationPath, "").Replace("\\", "/");
                fileDestination = options.DestinationPath + fileDestination;
                Upload(client, file, fileDestination);

                copied++;
            }
        }

        private static void Upload(SftpClient client, FileInfo file, string destination)
        {
            using (var stream = File.OpenRead(file.FullName))
            {
                client.UploadFile(stream, destination);
            }
        }
    }
}