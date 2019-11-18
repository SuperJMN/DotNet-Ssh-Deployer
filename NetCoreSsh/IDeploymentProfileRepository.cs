namespace DotNetSsh
{
    public interface IDeploymentProfileRepository
    {
        void Add(DeploymentProfile profile);
        void Delete(DeploymentProfile profile);
    }
}