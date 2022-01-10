using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace DotNetSsh
{
    public interface IDeployer
    {
        Task<Result> Deploy(DirectoryInfo source, Deployment settings);
    }
}