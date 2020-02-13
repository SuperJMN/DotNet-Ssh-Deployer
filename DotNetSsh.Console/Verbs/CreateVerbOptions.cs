using CommandLine;

namespace DotNetSsh.Console.Verbs
{
    [Verb("create")]
    public class CreateVerbOptions
    {
        [Option("name", Required = true, HelpText = "Profile name")]
        public string Name { get; set; }

        [Option("target-device", Required = true)]
        public TargetDevice TargetDevice { get; set; }

        [Option('p', "project", Required = false, HelpText = "Project name")]
        public string Project { get; set; }

        [Option('u', "username", Required = false, HelpText = "Sets the username of the template")]
        public string UserName { get; set; }

        [Option('p', "password", Required = false, HelpText = "Sets the password for the template")]
        public string Password { get; set; }

        [Option('h', "host-name", Required = false, HelpText = "Sets the hostname")]
        public string Host { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}