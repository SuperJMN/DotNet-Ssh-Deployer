using System.IO;
using CSharpFunctionalExtensions;

namespace DotNetSsh
{
    public interface IProjectPublisher
    {
        Result<DirectoryInfo> Publish(string projectPath, Architecture device, string framework, string configuration);
    }
}