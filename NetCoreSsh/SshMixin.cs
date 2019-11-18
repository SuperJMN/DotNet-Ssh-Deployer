using System.IO;
using Renci.SshNet;
using Serilog;

namespace DotNetSsh
{
    public static class SshMixin
    {
        public static void MirrorDirTree(this SshClient client, string destination, string source)
        {
            CreateDirectory(client, destination);

            var subDirsInParent = Directory.GetDirectories(source, "*.*", SearchOption.AllDirectories);
            foreach (var subdirInParent in subDirsInParent)
            {
                var transformToDest = subdirInParent.Replace(source, "").Replace("\\", "/");
                var finalDir = destination + transformToDest;
                CreateDirectory(client, finalDir);
            }
        }

        public static void CreateDirectory(this SshClient clientsSshClient, string destinationPath)
        {
            Log.Verbose("Creating destination directory {Directory}", destinationPath);
            clientsSshClient.RunCommand($"mkdir -p {destinationPath}");
        }

        public static void DeleteExisting(this SshClient sshClient, string path)
        {
            Log.Verbose("Deleting previous {Directory}", path);

            sshClient.RunCommand($"rm -rf {path}");
        }

        public static void Copy(this SftpClient client, string source, string destination)
        {
            var dir = new DirectoryInfo(source);
            var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

            int total = files.Length;
            int copied = 0;
            foreach (var file in files)
            {
                var percent = (float)copied / total;
                Log.Verbose("{Percentage:P}", percent);

                var fileDestination = file.FullName.Replace(source, "").Replace("\\", "/");
                fileDestination = destination + fileDestination;
                Upload(client, file, fileDestination);

                copied++;
            }
        }

        private static void Upload(SftpClient client, FileSystemInfo file, string destination)
        {
            using (var stream = File.OpenRead(file.FullName))
            {
                client.UploadFile(stream, destination);
            }
        }
    }
}