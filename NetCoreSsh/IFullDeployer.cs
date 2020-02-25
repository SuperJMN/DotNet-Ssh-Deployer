using System.Threading.Tasks;

namespace DotNetSsh
{
    public interface IFullDeployer
    {
        Task Deploy(Deployment settings);
    }
}