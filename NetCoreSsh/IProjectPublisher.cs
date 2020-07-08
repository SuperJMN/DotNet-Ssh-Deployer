using System.IO;

namespace DotNetSsh
{
    public interface IProjectPublisher
    {
        DirectoryInfo Publish(string projectPath, string device, string framework, string configuration);
    }
}