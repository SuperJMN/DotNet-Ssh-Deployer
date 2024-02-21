using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Renci.SshNet;

namespace DotNetSsh
{
    public class FolderLookup : Dictionary<string, byte[]>
    {
        private static readonly SHA1CryptoServiceProvider HashProvider = new SHA1CryptoServiceProvider();

        private FolderLookup(IDictionary<string, byte[]> dict) : base(dict)
        {
        }

        public static async Task<FolderLookup> FromDirectory(DirectoryInfo path)
        {
            var files = path.GetFiles("*.*", SearchOption.AllDirectories);
            
            var observable = files.ToObservable(new TaskPoolScheduler(new TaskFactory(TaskScheduler.Default)))
                .SelectMany(fi =>
                    Observable
                        .Using(fi.OpenRead,
                            stream =>
                            {
                                var hash = HashProvider.ComputeHash(stream);
                                return Observable.Return(new Lookup(path.ConvertToRelative(fi), hash));
                            }));


            var lookups = await observable.ToList();
            var dict = lookups.ToDictionary(lookup => lookup.Name, lookup => lookup.Hash);
            return new FolderLookup(dict);
        }

        private class Lookup
        {
            public string Name { get; }
            public byte[] Hash { get; }

            public Lookup(string name, byte[] hash)
            {
                Name = name;
                Hash = hash;
            }
        }

        public static async Task<FolderLookup> FromFile(SftpClient client, string path)
        {
            var dict = new Dictionary<string, byte[]>();
            await using var stream = new MemoryStream();
            if (client.Exists(path))
            {
                client.DownloadFile(path, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);

                var contents = await reader.ReadToEndAsync();
                dict = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(contents);
            }
            
            return new FolderLookup(dict);
        }
    }
}