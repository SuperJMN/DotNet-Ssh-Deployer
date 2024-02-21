using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace DotNetSsh
{
    public static class ProjectMetadataMixin
    {
        private const string TargetFramework = "/Project/PropertyGroup/TargetFramework";
        private const string TargetFrameworks = "/Project/PropertyGroup/TargetFrameworks";
        private const string AssemblyName = "/Project/PropertyGroup/AssemblyName";
        private const string UserSecretsId = "/Project/PropertyGroup/UserSecretsId";

        public static string GetOutputPath(XPathNavigator nav, string configName = "Release|AnyCPU")
        { 
            var xPath =
                $@"/Project/PropertyGroup[@Condition=""'$(Configuration)|$(Platform)'=='{configName}'""]/OutputPath";
            var outputPathNode = nav.SelectSingleNode(xPath);
            return outputPathNode?.InnerXml;
        }

        public static IEnumerable<string> GetFrameworks(XPathNavigator nav)
        {
            var framework = nav.SelectSingleNode(TargetFramework);
            if (framework != null)
            {
                return new[] { framework.InnerXml };
            }

            var frameworks = nav.SelectSingleNode(TargetFrameworks);

            if (frameworks != null) return frameworks.InnerXml.Split(';');

            throw new XmlException("Could not find TargetFramework/s entry in the project definition");
        }

        public static string GetAssemblyName(XPathNavigator nav)
        {
            var node = nav.SelectSingleNode(AssemblyName);

            var assemblyName = node?.InnerXml;
            return assemblyName;
        }

        public static string GetUserSecretsId(XPathNavigator nav)
        {
            var node = nav.SelectSingleNode(UserSecretsId);

            var assemblyName = node?.InnerXml;
            return assemblyName;
        }
    }
}