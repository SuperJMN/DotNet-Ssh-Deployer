using System.Collections.Generic;

namespace DotNetSsh
{
    public interface IDeploymentProfileRepository
    {
        void AddOrUpdate(DeploymentProfile profile);
        void Delete(string name);
        IEnumerable<DeploymentProfile> GetAll();
        DeploymentProfile Get(string name);
    }
}