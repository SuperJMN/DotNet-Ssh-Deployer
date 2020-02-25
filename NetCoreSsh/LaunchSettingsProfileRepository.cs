using System.Collections.Generic;
using System.IO;
using DotNetSsh.LaunchSettings;
using DotNetSsh.LaunchSettings.MyNamespace;
using Newtonsoft.Json;

namespace DotNetSsh
{
    public class LaunchSettingsProfileRepository : ILaunchSettingsProfileRepository
    {
        private readonly IFileSystem fileSystem;
        private readonly LaunchSettingsRoot launchSettingsRoot;
        private readonly string filePath;

        public LaunchSettingsProfileRepository(string projectPath, IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            filePath = Path.Combine(Path.GetDirectoryName(projectPath), "Properties", "launchsettings.json");
            launchSettingsRoot = Load();
        }

        public Profile Get(string name)
        {
            return launchSettingsRoot.Profiles[name];
        }

        public void AddOrUpdate(string name, Profile profile)
        {
            launchSettingsRoot.Profiles[name] = profile;
            Save();
        }

        private LaunchSettingsRoot Load()
        {
            if (!File.Exists(filePath))
            {
                return new LaunchSettingsRoot
                {
                    Profiles = new Dictionary<string, Profile>()
                };
            }

            var contents = File.ReadAllText(filePath);
            var root = JsonConvert.DeserializeObject<LaunchSettingsRoot>(contents);

            return root;
        }

        private void Save()
        {
            var serialized = JsonConvert.SerializeObject(launchSettingsRoot, Formatting.Indented);

            fileSystem.EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, serialized);
        }
    }
}