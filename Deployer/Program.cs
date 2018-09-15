using System;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using CommandLine;
using Renci.SshNet;
using Serilog;

namespace Deployer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();
            
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(Execute);
        }

        private static void Execute(CommandLineOptions o)
        {
            Console.WriteLine(".NET SSH/SFTP Deployment Utility for NET Core");
            Log.Information("Starting deployment...");

            try
            {
                SetDefaultsForEmptyOptions(o);

                var options = new DeploymentOptions
                {
                    Project = o.Project,
                    DestinationPath = o.Destination,
                    Host = o.Host,
                    Credentials = new Credentials
                    {
                        User = o.UserName,
                        Password = o.Password,
                    }
                };

                Deploy(options);
            }
            catch (Exception e)
            {
                Log.Error(e, "Deployment failed");
            }
        }

        private static void SetDefaultsForEmptyOptions(CommandLineOptions o)
        {
            if (o.Project == null)
            {
                var defaultProject = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj").FirstOrDefault();
                o.Project = defaultProject ??
                            throw new InvalidOperationException("No project to deploy. Please, specify a project.");

                Log.Verbose("No project file has been specified, taking {CsProj} from current folder", defaultProject);
            }

            if (o.Destination == null)
            {
                var projectName = Path.GetFileNameWithoutExtension(o.Project);
                var defaultDestination = "/home/pi/DotNet" + "/" + projectName;
                o.Destination = defaultDestination;
                Log.Verbose("No destination folder has been specified, taking default: {CsProj}", defaultDestination);
            }
        }

        private static void Deploy(DeploymentOptions options)
        {
            Log.Information("Working...");

            Publish(options);
            RemoteOperations(options);

            Log.Information("Deployment done!");
        }

        private static void RemoteOperations(DeploymentOptions options)
        {
            var sftp = new SftpClient(options.Host, options.Credentials.User, options.Credentials.Password);
            var ssh = new SshClient(options.Host, options.Credentials.User, options.Credentials.Password);

            using (var clients = new Clients(sftp, ssh))
            {
                clients.Connect();

                SyncFiles(options, clients);
                GiveExecutablePermission(options, clients);
            }
        }

        private static void GiveExecutablePermission(DeploymentOptions options, Clients clients)
        {
            var executable = options.DestinationPath + "/" + options.AssemblyName;
            clients.SshClient.RunCommand($"chmod +x {executable}");
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

            PrepareOutputDirectory(options.DestinationPath, options.PublishFolder, clients.SshClient);

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
            var dir = new DirectoryInfo(options.PublishFolder);
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

            int total = files.Length;
            int copied = 0;
            foreach (var file in files)
            {
                var percent = (float)copied / total;
                Log.Verbose("{Percentage:P}", percent);

                var fileDestination = file.FullName.Replace(options.PublishFolder, "").Replace("\\", "/");
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

        private static void Publish(DeploymentOptions options)
        {
            Log.Information("Building project {Project}...", options.Project);
            var parameters = $@"publish ""{options.Project}"" --configuration Release -r linux-arm";
            var cmd = "dotnet";
            ProcessUtils.Run(cmd, parameters);
        }
    }
}