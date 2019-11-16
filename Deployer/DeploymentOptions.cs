using System.IO;
using System.Xml.XPath;

namespace SshDeploy
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
                var project = Project;
                return GetPublishFolder(project);
            }
        }

        private string GetPublishFolder(string project)
        {
            var explicitPath = SshDeploy.Project.GetOutputPath(project);
            var implicitPath = Path.Combine("bin", "Release", SshDeploy.Project.GetFramework(project));

            var root = explicitPath ?? implicitPath;

            return Path.Combine(Source, root, "linux-arm", "publish");
        }

        public string Project { get; set; }

        public string Framework { get; set; }
        public string Display { get; set; }
        public bool RunAfterDeployment { get; set; }
    }
}