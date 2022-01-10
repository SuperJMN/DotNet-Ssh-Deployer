using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace DotNetSsh
{
    public class DeploymentUnit
    {
        private readonly IFullDeployer deployer;
        private readonly Deployment deployment;

        public DeploymentUnit(IFullDeployer deployer, Deployment deployment)
        {
            this.deployer = deployer;
            this.deployment = deployment;
        }

        public Task<Result> Deploy()
        {
            return deployer.Deploy(deployment);
        }
    }
}