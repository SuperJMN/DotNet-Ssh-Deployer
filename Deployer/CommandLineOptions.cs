using CommandLine;

namespace Deployer
{
    public class CommandLineOptions
    {
        [Option('s', "source-project", Required = false, HelpText = "Source project")]
        public string Project { get; set; }

        [Option('d', "destination", Required = false, HelpText = "Full Destination folder")]
        public string Destination { get; set; }

        [Option('h', "host", Required = false, HelpText = "Your target device host", Default = "raspberrypi")]
        public string Host { get; set; }

        [Option('u', "username", Required = false, HelpText = "User name", Default = "pi")]
        public string UserName { get; set; }

        [Option('p', "password", Required = false, HelpText = "Password", Default = "raspberry")]
        public string Password { get; set; }
    }
}