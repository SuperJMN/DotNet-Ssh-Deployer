using CommandLine;

namespace NetCoreSsh
{
    [Verb("create-template")]
    public class CreateTemplateOptions
    {
        [Option('t', "target-device", Required = true, HelpText = "Specified the target template to create")]
        public TargetDevice Target { get; set; }

        [Option('u', "username", Required = false, HelpText = "Sets the username of the template")]
        public string UserName { get; set; }

        [Option('u', "password", Required = false, HelpText = "Sets the password for the template")]
        public string Password { get; set; }

        [Option('h', "host-name", Required = false, HelpText = "Sets the hostname")]
        public string Host { get; set; }
    }
}