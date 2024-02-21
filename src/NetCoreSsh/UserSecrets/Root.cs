using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSsh.UserSecrets
{
    public class Root
    {

        [JsonProperty("SshDeployer")]
        public IList<Profile> Profiles { get; set; }
    }
}