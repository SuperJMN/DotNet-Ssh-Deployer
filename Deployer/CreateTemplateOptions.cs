using CommandLine;

namespace SshDeploy
{
    [Verb("create-template")]
    public class CreateTemplateOptions
    {
        [Option('p', "project-name", Required = true, HelpText = "Project name. Will affect the destination folder in the deployment machine.")]
        public string ProjectName { get; set; }
    }
}