using System.IO;
using DotNetSsh.UserSecrets;
using Renci.SshNet;
using Xunit;

namespace DotNetSsh.Tests
{
    public class UserSecretsTest
    {
        [Fact]
        public void List()
        {
            var sut = new UserSecretsManager("D:\\Repos\\SuperJMN-Zafiro\\AvaloniaDesigner\\AvaloniaDesigner\\Designer.csproj");
            var list = sut.List();
        }

        [Fact]
        public void Set()
        {
            var sut = new UserSecretsManager("D:\\Repos\\SuperJMN-Zafiro\\AvaloniaDesigner\\AvaloniaDesigner\\Designer.csproj");
            sut.Set("MySecret", "My value");
        }
    }
}