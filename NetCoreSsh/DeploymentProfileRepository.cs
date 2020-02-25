using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace DotNetSsh
{
    public class DeploymentProfileRepository : IDeploymentProfileRepository
    {
        private const string ProfileStoreFilename = "ssh-deployment.json";

        private readonly string storeFile;
        private readonly IDictionary<string, CustomizableSettings> dict;
        private readonly JsonSerializerSettings jsonSerializerSettings;

        private IEnumerable<DeploymentProfile> Profiles => dict.Select(x => new DeploymentProfile(x.Key, x.Value));

        public DeploymentProfileRepository(string projectFile)
        {
            storeFile = Path.Combine(Path.GetDirectoryName(projectFile), ProfileStoreFilename);
            dict = GetDict();

            jsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

        private IDictionary<string, CustomizableSettings> GetDict()
        {
            if (File.Exists(storeFile))
            {
                var p = JsonConvert.DeserializeObject<Profiles>(File.ReadAllText(storeFile), jsonSerializerSettings);
                return p.ToDictionary(x => x.Name, x => x.Settings);
            }

            return new Dictionary<string, CustomizableSettings>();
        }

        public void AddOrUpdate(DeploymentProfile profile)
        {
            dict[profile.Name] = profile.Settings;
            Save();
        }

        private void Save()
        {
            if (!File.Exists(storeFile))
            {
                Log.Verbose($"'{storeFile}' doesn't exist and it will be created.");
            }
            
            File.WriteAllText(storeFile, JsonConvert.SerializeObject(Profiles, jsonSerializerSettings));
        }

        // ReSharper disable once UnusedMember.Global
        public void Delete(string name)
        {
            dict.Remove(name);
            Save();
        }

        public IEnumerable<DeploymentProfile> GetAll()
        {
            return Profiles;
        }

        public DeploymentProfile Get(string name)
        {
            var existing = Profiles.FirstOrDefault(x => x.Name == name);
            return existing;
        }
    }
}