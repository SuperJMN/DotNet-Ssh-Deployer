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

        public string Project { get; set; }
    }
}