using System;
using System.IO;
using System.Linq;
using System.Net;

namespace NetCoreSsh
{
    public class DeploymentOptionsBuilder
    {
        private string assemblyName;
        private Credentials credentials = new Credentials() { User = "{user}", Password = "{password}"};
        private string projectPath;
        private string framework;
        private string runtime;
        private bool runAfterDeployment = true;
        private string host = "{host}";
        private string display  = ":0.0";

        public DeploymentOptionsBuilder()
        {
            Destination = new DestinationTarget(this);
        }

        public DeploymentOptions Build()
        {
            ProjectMetadata projectMetadata = null;
            if (projectPath != null)
            {
                projectMetadata = ProjectMetadata.FromPath(projectPath);
            }

            assemblyName = assemblyName ?? projectMetadata?.AssemblyName ?? Path.GetFileNameWithoutExtension(projectPath);
            return new DeploymentOptions
            {
                AssemblyName = assemblyName,
                Framework = framework ?? projectMetadata?.Frameworks.FirstOrDefault(),
                Credentials = credentials,
                Runtime = runtime,
                DestinationPath = Destination.Value,
                RunAfterDeployment = runAfterDeployment,
                Display = display,
                Host = host,
            };
        }

        public DeploymentOptionsBuilder FromProject(string path)
        {
            projectPath = path;

            return this;
        }

        public DestinationTarget Destination { get; }

        public DeploymentOptionsBuilder ForDevice(TargetDevice device)
        {
            runtime = GetRuntime(device);
            return this;
        }

        private static string GetRuntime(TargetDevice templateOptions)
        {
            switch (templateOptions)
            {
                case TargetDevice.Raspbian:
                    return "linux-arm";
                case TargetDevice.GenericLinux64:
                    return "linux-x64";
                default:
                    throw new ArgumentOutOfRangeException(nameof(templateOptions), templateOptions, null);
            }
        }

        public class DestinationTarget
        {
            private readonly DeploymentOptionsBuilder builder;

            public string Value => ShouldUseCredentials ? $"/home/{builder.credentials.User}/DotNetApps/{builder.assemblyName}" : "/home/{someFolder}";

            public DestinationTarget(DeploymentOptionsBuilder builder)
            {
                this.builder = builder;
            }

            public DeploymentOptionsBuilder UseCredentials()
            {
                ShouldUseCredentials = true;
                return builder;
            }

            public bool ShouldUseCredentials { get; private set; }
        }
    }
}