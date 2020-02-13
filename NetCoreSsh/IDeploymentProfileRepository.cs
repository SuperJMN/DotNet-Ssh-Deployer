using System.Collections.Generic;

namespace DotNetSsh
{
    public interface IDeploymentProfileRepository
    {
        void Add(DeploymentProfile profile);
        void Delete(DeploymentProfile profile);
        IEnumerable<DeploymentProfile> GetAll();
        DeploymentProfile Get(string name);
    }
}