using CommandLine;
using NetCoreSsh;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AddVerbOptions, DeployVerbOptions>(args)
                .WithParsed((AddVerbOptions o) => AddOrReplaceProfile(o))
                .WithParsed((DeployVerbOptions o) => Deploy(o));
        }

        private static void Deploy(DeployVerbOptions verbOptions)
        {
            var repo = new DeploymentProfileRepository("deployment.json");
            var profile = repo.Get(verbOptions.Name);

            var deployer = new SshDeployer();
            var publisher = new ProjectPublisher();

            deployer.Deploy(profile.Options);
        }

        private static void AddOrReplaceProfile(AddVerbOptions verbOptions)
        {
            DeploymentOptions ops;
            if (verbOptions.Project != null)
            {
                ops = new DeploymentOptionsBuilder()
                    .ForDevice(verbOptions.Runtime)
                    .FromProject(verbOptions.Project)
                    .Build();
            }
            else
            {
                ops = new DeploymentOptionsBuilder()
                    .ForDevice(verbOptions.Runtime)
                    .Build();
            }

            var repo = new DeploymentProfileRepository("deployment.json");
            repo.Add(new DeploymentProfile(verbOptions.Name, ops));
        }
    }
}
