using CommandLine;

namespace ConsoleApp1
{
    [Verb("deploy")]
    public class DeployVerbOptions
    {
        [Option("name", Required = true, HelpText = "Profile name")]
        public string Name { get; set; }
    }
}