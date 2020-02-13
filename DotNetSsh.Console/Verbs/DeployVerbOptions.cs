using CommandLine;

namespace DotNetSsh.Console.Verbs
{
    [Verb("deploy")]
    public class DeployVerbOptions
    {
        [Option('p', "profile", Required = true, HelpText = "Profile name")]
        public string Name { get; set; }

        [Option('s', "source", Required = false, HelpText = "Project file to deploy. If no project is specified, the project from the current folder will be used.")]
        public string ProjectFile { get; set; }

        [Option('w',"wipe", Required = false, HelpText = "Wipe (clean) deployment folder before deployment.", Default = false)]
        public bool CleanTarget { get; set; }

        [Option('c', "configuration", Required = false, Default = "Debug", HelpText = "Configuration name, usually 'Debug' or 'Release'")]
        public string Configuration { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}