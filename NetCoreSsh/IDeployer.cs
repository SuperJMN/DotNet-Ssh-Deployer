using System.IO;
using System.Threading.Tasks;

namespace DotNetSsh
{
    public interface IDeployer
    {
        Task Deploy(DirectoryInfo source, Deployment settings);
    }
}