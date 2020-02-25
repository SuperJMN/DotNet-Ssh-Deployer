using System.Threading.Tasks;

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

        public Task Deploy()
        {
            return deployer.Deploy(deployment);
        }
    }
}