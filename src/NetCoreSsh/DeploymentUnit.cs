using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotNetSsh.UserSecrets;

namespace DotNetSsh
{
    public class DeploymentUnit
    {
        private readonly IFullDeployer deployer;
        private readonly Deployment deployment;
        private readonly CredentialsManager credentialsManager;

        public DeploymentUnit(IFullDeployer deployer, Deployment deployment, CredentialsManager credentialsManager)
        {
            this.deployer = deployer;
            this.deployment = deployment;
            this.credentialsManager = credentialsManager;
        }

        public Task<Result> Deploy()
        {
            return deployer.Deploy(deployment, credentialsManager);
        }
    }
}