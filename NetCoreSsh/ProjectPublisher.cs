using System;
using System.IO;
using Serilog;

namespace DotNetSsh
{
    public class ProjectPublisher : IProjectPublisher
    {
        public DirectoryInfo Publish(string projectPath, TargetDevice device, string framework, string configuration)
        {
            var publishPath = GetPublishPath(projectPath, device, framework, configuration);
            Log.Information("Publishing project {Project} for {TargetDevice} to path {Path}...", projectPath, device, publishPath);
            var parameters = $@"publish ""{projectPath}"" --configuration {configuration} -r {GetRuntime(device)} -f {framework}";
            var cmd = "dotnet";
            ProcessUtils.Run(cmd, parameters);
            
            return publishPath;
        }

        private static DirectoryInfo GetPublishPath(string pathToProject, TargetDevice device, string framework, string configuration)
        {
            var metadata = ProjectMetadata.FromPath(pathToProject);
            var implicitPath = Path.Combine("bin", configuration, framework);
            var outputPath = metadata.OutputPath ?? implicitPath;

            return new DirectoryInfo(Path.Combine(Path.GetDirectoryName(pathToProject), outputPath, GetRuntime(device), "publish"));
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
    }
}