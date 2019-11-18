using System;
using System.IO;
using Serilog;

namespace DotNetSsh.Console
{
    internal class ProjectPublisher
    {
        private const string Configuration = "Release";

        public string Publish(string projectPath, TargetDevice device, string framework)
        {
            Log.Information("Building project {Project}...", projectPath);
            var parameters = $@"publish ""{projectPath}"" --configuration {Configuration} -r {GetRuntime(device)} -f {framework}";
            var cmd = "dotnet";
            ProcessUtils.Run(cmd, parameters);

            return GetPublishPath(projectPath, device, framework);
        }

        private static string GetPublishPath(string pathToProject, TargetDevice device, string framework)
        {
            var metadata = ProjectMetadata.FromPath(pathToProject);
            var implicitPath = Path.Combine("bin", Configuration, framework);
            var outputPath = metadata.OutputPath ?? implicitPath;

            return Path.Combine(Path.GetDirectoryName(pathToProject), outputPath, GetRuntime(device), "publish");
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