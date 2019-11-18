using CommandLine;

namespace DotNetSsh.Console
{
    [Verb("deploy")]
    public class DeployVerbOptions
    {
        [Option("name", Required = true, HelpText = "Profile name")]
        public string Name { get; set; }

        [Option("project", Required = false, HelpText = "Project file")]
        public string ProjectFile { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}