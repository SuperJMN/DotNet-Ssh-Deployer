using System.Threading.Tasks;
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

        public async Task Deploy(Deployment settings)
        {
            Log.Information("Operation started");
            var publishDirectory = publisher.Publish(settings.ProjectPath, settings.Settings.Architecture, settings.Settings.Framework,
                settings.BuildConfiguration);
            await deployer.Deploy(publishDirectory, settings);
            Log.Information("Operation finished");
        }
    }
}