using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog;

namespace DotNetSsh
{
    public class DeploymentProfileRepository : IDeploymentProfileRepository
    {
        private readonly string filePath;
        private readonly Profiles profiles;

        public DeploymentProfileRepository(string filePath)
        {
            this.filePath = filePath;
            profiles = Load();
        }

        private Profiles Load()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    return JsonConvert.DeserializeObject<Profiles>(File.ReadAllText(filePath));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Couldn't load file {Path}", filePath);
                }
            }

            return new Profiles();
        }

        public void Add(DeploymentProfile profile)
        {
            var existing = profiles.FirstOrDefault(x => x.Name == profile.Name);
            if (existing != null)
            {
                profiles.Remove(existing);
            }

            profiles.Add(profile);
            Save();
        }

        private void Save()
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(profiles));
        }

        public void Delete(DeploymentProfile profile)
        {
            profiles.Remove(profile);
            Save();
        }

        public DeploymentProfile Get(string name)
        {
            var existing = profiles.FirstOrDefault(x => x.Name == name);
            return existing;
        }
    }
}