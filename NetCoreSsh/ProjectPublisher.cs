using System;
using System.IO;
using Serilog;

namespace DotNetSsh
{
    public class ProjectPublisher : IProjectPublisher
    {
        public DirectoryInfo Publish(string projectPath, Architecture device, string framework, string configuration)
        {
            var publishPath = GetPublishPath(projectPath, device, framework, configuration);
            Log.Information("Publishing project {Project} with runtime {Architecture} to path {Path}...", projectPath, device, publishPath);
            var parameters = $@"publish ""{projectPath}"" --configuration {configuration} -r {GetRuntime(device)} -f {framework}";
            var cmd = "dotnet";
            ProcessUtils.Run(cmd, parameters);
            
            return publishPath;
        }

        private static DirectoryInfo GetPublishPath(string pathToProject, Architecture device, string framework, string configuration)
        {
            var metadata = ProjectMetadata.FromPath(pathToProject);
            var implicitPath = Path.Combine("bin", configuration, framework);
            var outputPath = metadata.OutputPath ?? implicitPath;

            return new DirectoryInfo(Path.Combine(Path.GetDirectoryName(pathToProject), outputPath, GetRuntime(device), "publish"));
        }

        private static string GetRuntime(Architecture templateOptions)
        {
            switch (templateOptions)
            {
                case Architecture.LinuxArm32:
                    return "linux-arm";
                case Architecture.LinuxArm64:
                    return "linux-arm64";
                case Architecture.Linux64:
                    return "linux-x64";
                default:
                    throw new ArgumentOutOfRangeException(nameof(templateOptions), templateOptions, null);
            }
        }
    }
}