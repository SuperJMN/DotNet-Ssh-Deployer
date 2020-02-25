using System.IO;

namespace DotNetSsh
{
    public interface IProjectPublisher
    {
        DirectoryInfo Publish(string projectPath, TargetDevice device, string framework, string configuration);
    }
}