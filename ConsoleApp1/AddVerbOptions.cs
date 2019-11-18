using CommandLine;
using NetCoreSsh;

namespace ConsoleApp1
{
    [Verb("create")]
    public class AddVerbOptions
    {
        [Option("name", Required = true, HelpText = "Profile name")]
        public string Name { get; set; }

        [Option("runtime", Required = true)]
        public TargetDevice Runtime { get; set; }

        [Option('p', "project", Required = false, HelpText = "Project name")]
        public string Project { get; set; }

        [Option('u', "username", Required = false, HelpText = "Sets the username of the template")]
        public string UserName { get; set; }

        [Option('p', "password", Required = false, HelpText = "Sets the password for the template")]
        public string Password { get; set; }

        [Option('h', "host-name", Required = false, HelpText = "Sets the hostname")]
        public string Host { get; set; }
    }
}