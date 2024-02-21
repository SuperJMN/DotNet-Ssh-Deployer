using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotNetSsh.UserSecrets;

namespace DotNetSsh
{
    public interface IDeployer
    {
        Task<Result> Deploy(DirectoryInfo source, Deployment settings, CredentialsManager credentialsManager);
    }
}