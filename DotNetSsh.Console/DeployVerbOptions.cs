using CommandLine;

namespace DotNetSsh.Console
{
    [Verb("deploy")]
    public class DeployVerbOptions
    {
        [Option('p', "profile", Required = true, HelpText = "Profile name")]
        public string Name { get; set; }

        [Option('s', "source", Required = false, HelpText = "Project file to deploy. If no project is specified, the project from the current folder will be used.")]
        public string ProjectFile { get; set; }

        [Option('c',"clean", Required = false, HelpText = "Clean (delete) deployment folder before deployment.", Default = false)]
        public bool CleanTarget { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}