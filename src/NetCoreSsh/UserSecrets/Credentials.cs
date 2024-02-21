using Newtonsoft.Json;

namespace DotNetSsh.UserSecrets
{
    public class Credentials
    {

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }
    }
}