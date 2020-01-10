using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByteSizeLib;
using Newtonsoft.Json;
using Renci.SshNet;
using Serilog;

namespace DotNetSsh
{
    internal class SshFolderSynchronizer
    {
        private const string HashStoreFilename = "folder-hashes.json";
        private readonly SftpClient client;

        public SshFolderSynchronizer(SftpClient client)
        {
            this.client = client;
        }

        public async Task Sync(DirectoryInfo source, string destination)
        {
            var filename = destination + "/" + HashStoreFilename;

            Log.Verbose("Creating hashes for current deployment...");
            var localFilesLookup = await FolderLookup.FromDirectory(source);
            Log.Verbose("Retrieving previous deployment hashes...");
            var remoteFilesLookup = await FolderLookup.FromFile(client, filename);

            var filesToCopy = GetFilesToCopy(localFilesLookup, remoteFilesLookup);

            var files = filesToCopy.Select(path => new FileInfo(Path.Join(source.FullName, path)));
            client.MirrorDirTree(source, destination);
            CopyFiles(source, files.ToList(), destination);

            SaveLocalLookup(localFilesLookup, filename);
        }

        private IEnumerable<string> GetFilesToCopy(FolderLookup local, FolderLookup remote)
        {
            var lookup = local.Concat(remote).ToLookup(x => x.Key, pair => pair.Value);

            var bothExist = lookup
                .Where(pairs => pairs.Count() == 2);

            var hashMismatch = bothExist
                .Where(hashes =>
                {
                    var list = hashes.ToList();
                    var localHash = list[0];
                    var remoteHash = list[1];
                    return !localHash.SequenceEqual(remoteHash);
                })
                .Select(x => x.Key);

            var localOnly = local.Where(pair => !remote.ContainsKey(pair.Key)).Select(x => x.Key);
            var filesToCopy = hashMismatch.Concat(localOnly);

            return filesToCopy;
        }

        private void SaveLocalLookup(IDictionary<string, byte[]> lookup, string filename)
        {
            var serializeObject = JsonConvert.SerializeObject(lookup, Formatting.Indented);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializeObject));
            client.UploadFile(stream, filename);
        }

        private void CopyFiles(DirectoryInfo source, ICollection<FileInfo> files, string destination)
        {
            if (!files.Any())
            {
                Log.Information("The deployment folder is up to date");
                return;
            }

            var size = ByteSize.FromBytes(files.Sum(s => s.Length));
            Log.Information("Sending {Count} files ({Size}) to '{Destination}'", files.Count, size,destination);

            int total = files.Count;
            int copied = 0;
            foreach (var file in files)
            {
                var percent = (float) copied / total;
                Log.Verbose("{Percentage:P}", percent);
                Log.Verbose("Copying {File} ({Size})", file, ByteSize.FromBytes(file.Length));

                var fileDestination = source.ConvertToRelative(file).ToLinuxPath();
                fileDestination = destination + "/" + fileDestination;

                Copy(file, fileDestination);

                copied++;
            }
        }

        private void Copy(FileSystemInfo file, string destination)
        {
            using var stream = File.OpenRead(file.FullName);
            client.UploadFile(stream, destination, true);
        }
    }
}