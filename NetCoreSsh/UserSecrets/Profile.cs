using Newtonsoft.Json;

namespace DotNetSsh.UserSecrets
{
    public class Profile
    {

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Credentials")]
        public Credentials Credentials { get; set; }
    }
}