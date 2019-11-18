using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;

namespace NetCoreSsh
{
    public class ProjectMetadataMixin
    {
        public static string GetOutputPath(XPathNavigator nav, string configName = "Release|AnyCPU")
        { 
            var xPath =
                $@"/Project/PropertyGroup[@Condition=""'$(Configuration)|$(Platform)'=='{configName}'""]/OutputPath";
            var outputPathNode = nav.SelectSingleNode(xPath);
            return outputPathNode?.InnerXml;
        }

        public static IEnumerable<string> GetFrameworks(XPathNavigator nav)
        {
            var framework = nav.SelectSingleNode("/Project/PropertyGroup/TargetFramework");
            if (framework != null)
            {
                return new[] { framework.InnerXml };
            }

            var frameworks = nav.SelectSingleNode("/Project/PropertyGroup/TargetFrameworks");

            return frameworks.InnerXml.Split(';');
        }

        public static string GetAssemblyName(XPathNavigator nav, string projectPath)
        {
            var node = nav.SelectSingleNode("/Project/PropertyGroup/AssemblyName");

            var assemblyName = node?.InnerXml;
            return assemblyName;
        }
    }
}