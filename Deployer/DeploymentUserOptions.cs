using CommandLine;
using Newtonsoft.Json;

namespace SshDeploy
{
    [Verb("deploy")]
    public class DeploymentUserOptions
    {
        [Option('s', "source-project", Required = false, HelpText = "Source project")]
        public string Project { get; set; }

        [Option('d', "destination", Required = false, HelpText = "Full Destination folder")]
        public string Destination { get; set; }

        [Option('h', "host", Required = false, HelpText = "Your target device host")]
        public string Host { get; set; }

        [Option('u', "username", Required = false, HelpText = "User name")]
        public string UserName { get; set; }

        [Option('p', "password", Required = false, HelpText = "Password")]
        public string Password { get; set; }

        [Option(HelpText = "Prints all messages to standard output", Default = false)]
        [JsonIgnore]
        public bool Verbose { get; set; }
    }
}