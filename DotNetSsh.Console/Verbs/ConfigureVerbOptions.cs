using CommandLine;

namespace DotNetSsh.Console.Verbs
{
    [Verb("configure")]
    public class ConfigureVerbOptions
    {
        [Option('p', "project", Required = false, HelpText = "Project name")]
        public string Project { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}