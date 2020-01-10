using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DotNetSsh.Tests
{
    public class FolderLookup
    {
        [Fact]
        public async Task Test1()
        {
            var sut = await DotNetSsh.FolderLookup.FromDirectory(new DirectoryInfo(Directory.GetCurrentDirectory()));
        }
    }
}
