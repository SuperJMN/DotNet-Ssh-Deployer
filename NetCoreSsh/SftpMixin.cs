using System.Diagnostics.CodeAnalysis;
using System.IO;
using Renci.SshNet;
using Serilog;

namespace DotNetSsh
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SftpMixin
    {
        public static void MirrorDirTree(this SftpClient client, DirectoryInfo source, string destination)
        {
            CreateDirectory(client, destination);

            var directories = source.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (var directory in directories)
            {
                var transformToDest = source.ConvertToRelative(directory);
                var finalDir = destination + "/" + transformToDest;
                CreateDirectory(client, finalDir);
            }
        }

        public static void CreateDirectory(this SftpClient clientsSshClient, string destinationPath)
        {
            if (clientsSshClient.Exists(destinationPath))
            {
                return;
            }

            Log.Verbose("Creating destination directory {Directory}", destinationPath);
            clientsSshClient.CreateDirectory(destinationPath);
        }

        public static void DeleteExisting(this SshClient sshClient, string path)
        {
            Log.Verbose("Deleting previous {Directory}", path);

            sshClient.RunCommand($"rm -rf {path}");
        }
    }
}