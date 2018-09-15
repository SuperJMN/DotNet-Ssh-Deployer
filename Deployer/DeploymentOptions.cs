using System.IO;
using System.Xml.XPath;

namespace Deployer
{
    internal class DeploymentOptions
    {
        public string Source => Path.GetDirectoryName(Project);
        public string DestinationPath { get; set; }
        public Credentials Credentials { get; set; }
        public string Host { get; set; }
        public string AssemblyName => GetAssemblyName(Project);

        private string GetAssemblyName(string projectPath)
        {
            var xml = new XPathDocument(projectPath);
            var nav = xml.CreateNavigator();
            var node = nav.SelectSingleNode("/Project/PropertyGroup/AssemblyName");

            if (node == null)
            {
                return Path.GetFileNameWithoutExtension(projectPath);
            }

            var assemblyName = node.InnerXml;
            return assemblyName;
        }

        public string PublishFolder
        {
            get
            {
                var xml = new XPathDocument(Project);
                var nav = xml.CreateNavigator();
                var node = nav.SelectSingleNode("/Project/PropertyGroup/TargetFramework");
                var targetFramework = node.InnerXml;

                var configName = "Release|AnyCPU";
                var xPath =
                    $@"/Project/PropertyGroup[@Condition=""'$(Configuration)|$(Platform)'=='{configName}'""]/OutputPath";
                var outputPathNode = nav.SelectSingleNode(xPath);

                string outputPath;
                if (outputPathNode == null)
                {
                    outputPath = Path.Combine("bin", "release");
                }
                else
                {
                    outputPath = outputPathNode.InnerXml;
                }

                var publishFolder = Path.Combine(Source, outputPath, targetFramework, "linux-arm", "publish");
                return publishFolder;
            }
        }

        public string Project { get; set; }
    }
}