using System.IO;
using System.Linq;

namespace DotNetSsh
{
    public class DeploymentOptionsBuilder
    {
        private string projectPath;
        private TargetDevice device = TargetDevice.GenericLinux64;

        public DeploymentOptions Build()
        {
            ProjectMetadata projectMetadata = null;
            if (projectPath != null)
            {
                projectMetadata = ProjectMetadata.FromPath(projectPath);
            }

            return new DeploymentOptions
            {
                AssemblyName = projectMetadata?.AssemblyName ?? Path.GetFileNameWithoutExtension(projectPath) ?? "{ExecutableName}",
                Framework = projectMetadata?.Frameworks.FirstOrDefault() ?? "{framework}",
                Credentials = new Credentials { User = "{user}", Password = "{password}" },
                TargetDevice = device,
                DestinationPath = "/home/{user}/DotNetApps/{Application}",
                RunAfterDeployment = true,
                Display = ":0.0",
                Host = "{host}",
            };
        }

        public DeploymentOptionsBuilder FromProject(string path)
        {
            projectPath = path;

            return this;
        }

        public DeploymentOptionsBuilder ForDevice(TargetDevice device)
        {
            this.device = device;
            return this;
        }
    }
}