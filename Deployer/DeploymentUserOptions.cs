using CommandLine;
using Newtonsoft.Json;

namespace SshDeploy
{
    [Verb("deploy")]
    public class DeploymentUserOptions
    {
        [Option('s', "source-project", Required = false, HelpText = "Source project")]
        public string ProjectName { get; set; }

        [Option('d', "destination", Required = false, HelpText = "Full Destination folder")]
        public string Destination { get; set; }

        [Option('h', "host", Required = false, HelpText = "Your target device host")]
        public string Host { get; set; }

        [Option('u', "username", Required = false, HelpText = "User name")]
        public string UserName { get; set; }

        [Option('p', "password", Required = false, HelpText = "Password")]
        public string Password { get; set; }

        [Option('f', "framework", Required = false, HelpText = "Target framework")]
        public string Framework { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        [JsonIgnore]
        public bool Verbose { get; set; }

        [Option(HelpText = "Selects the display for running the application")]
        public string Display { get; set; }

        [Option('a', "architecture", Required = false, HelpText = "Set the architecture (runtime)")]
        public string Runtime { get; set; }

        [Option('r', "run", Default = false)]
        public bool RunAfterDeployment { get; set; }
    }
}