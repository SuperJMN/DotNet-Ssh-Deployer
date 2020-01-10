using System.Collections.Generic;
using System.Xml.XPath;

namespace DotNetSsh
{
    public class ProjectMetadata
    {
        public static ProjectMetadata FromPath(string path)
        {
            var xml = new XPathDocument(path);
            var nav = xml.CreateNavigator();
            
            return new ProjectMetadata()
            {
                AssemblyName = ProjectMetadataMixin.GetAssemblyName(nav),
                Frameworks = ProjectMetadataMixin.GetFrameworks(nav),
                OutputPath = ProjectMetadataMixin.GetOutputPath(nav),
            };
        }

        public string OutputPath { get; private set; }

        public IEnumerable<string> Frameworks { get; private set; }

        public string AssemblyName { get; private set; }
    }
}