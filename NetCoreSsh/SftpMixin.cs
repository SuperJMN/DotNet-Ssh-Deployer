using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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

        public static void CreateDirectory(this SftpClient sftp, string destinationPath)
        {
            var abstractPath = new AbstractPath(destinationPath);
            Log.Verbose("Creating destination directory {Directory}", destinationPath);
            CreateDirectory(sftp, abstractPath);
        }

        private static void CreateDirectory(SftpClient sftp, AbstractPath directory)
        {
            if (directory.FullName == "")
            {
                return;
            }

            if (sftp.Exists(directory.SlashPath))
            {
                return;
            }

            CreateDirectory(sftp, directory.Parent);
            sftp.CreateDirectory(directory.SlashPath);
        }

        public static void DeleteExisting(this SshClient sshClient, string path)
        {
            Log.Verbose("Deleting previous {Directory}", path);

            sshClient.RunCommand($"rm -rf {path}");
        }
    }

    public class AbstractPath
    {
        private const string Backslash = "\\";
        private const string Slash = "/";
        private readonly string path;

        public AbstractPath(string path)
        {
            this.path = path.Replace("/", Backslash);
        }

        public IList<string> Parts => path.Split(Backslash);
        public AbstractPath Parent => new(string.Join(Backslash, Parts.SkipLast(1)));
        public string FullName => string.Join(Backslash, Parts);
        public string SlashPath => string.Join(Slash, Parts);
    }
}