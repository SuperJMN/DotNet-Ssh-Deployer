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
                AssemblyName = ProjectMetadataMixin.GetAssemblyName(nav, path),
                Frameworks = ProjectMetadataMixin.GetFrameworks(nav),
                OutputPath = ProjectMetadataMixin.GetOutputPath(nav),
            };
        }

        public string OutputPath { get; set; }

        public IEnumerable<string> Frameworks { get; set; }

        public string AssemblyName { get; set; }
    }
}