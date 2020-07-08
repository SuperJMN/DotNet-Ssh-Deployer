using Serilog;
using System.IO;

namespace DotNetSsh
{
    public class ProjectPublisher : IProjectPublisher
    {
        public DirectoryInfo Publish(string projectPath, string device, string framework, string configuration)
        {
            var publishPath = GetPublishPath(projectPath, device, framework, configuration);
            Log.Information("Publishing project {Project} for {TargetDevice} to path {Path}...", projectPath, device, publishPath);
            var parameters = $@"publish ""{projectPath}"" --configuration {configuration} -r {device} -f {framework}";
            var cmd = "dotnet";
            ProcessUtils.Run(cmd, parameters);

            return publishPath;
        }

        private static DirectoryInfo GetPublishPath(string pathToProject, string device, string framework, string configuration)
        {
            var metadata = ProjectMetadata.FromPath(pathToProject);
            var implicitPath = Path.Combine("bin", configuration, framework);
            var outputPath = metadata.OutputPath ?? implicitPath;

            return new DirectoryInfo(Path.Combine(Path.GetDirectoryName(pathToProject), outputPath, device, "publish"));
        }
    }
}