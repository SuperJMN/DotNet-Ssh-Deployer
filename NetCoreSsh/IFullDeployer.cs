using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotNetSsh.UserSecrets;

namespace DotNetSsh
{
    public interface IFullDeployer
    {
        Task<Result> Deploy(Deployment settings, CredentialsManager credentialsManager);
    }
}