using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DotNetSsh.UserSecrets;
using Serilog;

namespace DotNetSsh
{
    public class FullDeployer : IFullDeployer
    {
        private readonly IDeployer deployer;
        private readonly IProjectPublisher publisher;

        public FullDeployer(IDeployer deployer, IProjectPublisher publisher)
        {
            this.deployer = deployer;
            this.publisher = publisher;
        }

        public async Task<Result> Deploy(Deployment settings, CredentialsManager credentialsManager)
        {
            Log.Information("Operation started");
            var publishDirectory = publisher.Publish(settings.ProjectPath, settings.Settings.Architecture, settings.Settings.Framework,
                settings.BuildConfiguration);
            return await publishDirectory.Bind(dir => deployer.Deploy(dir, settings, credentialsManager));
        }
    }
}