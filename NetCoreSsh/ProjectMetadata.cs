using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;

namespace DotNetSsh
{
    public class ProjectMetadata
    {
        public static ProjectMetadata FromPath(string path)
        {
            var xml = new XPathDocument(path);
            var nav = xml.CreateNavigator();

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            return new ProjectMetadata()
            {
                ProjectName = fileNameWithoutExtension,
                AssemblyName = ProjectMetadataMixin.GetAssemblyName(nav) ?? fileNameWithoutExtension,
                Frameworks = ProjectMetadataMixin.GetFrameworks(nav),
                OutputPath = ProjectMetadataMixin.GetOutputPath(nav),
                UserSecretsId = ProjectMetadataMixin.GetUserSecretsId(nav),
            };
        }

        public string ProjectName { get; set; }

        public string UserSecretsId { get; set; }

        public string OutputPath { get; private set; }

        public IEnumerable<string> Frameworks { get; private set; }

        public string AssemblyName { get; private set; }
    }
}