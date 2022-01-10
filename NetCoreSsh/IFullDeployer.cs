using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace DotNetSsh
{
    public interface IFullDeployer
    {
        Task<Result> Deploy(Deployment settings);
    }
}