using System.Collections;

namespace NetCoreSsh
{
    public interface IDeploymentProfileRepository
    {
        void Add(DeploymentProfile profile);
        void Delete(DeploymentProfile profile);
    }
}