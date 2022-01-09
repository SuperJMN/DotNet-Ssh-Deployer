using System.IO;

namespace DotNetSsh
{
    public interface IProjectPublisher
    {
        DirectoryInfo Publish(string projectPath, Architecture device, string framework, string configuration);
    }
}