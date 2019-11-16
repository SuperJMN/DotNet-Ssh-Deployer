using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;

namespace SshDeploy
{
    internal class Project
    {
        public static string GetFramework(string project)
        {
            var frameworks = GetFrameworks(project);
            return frameworks.Where(s => s.StartsWith("netcoreapp"))
                .OrderByDescending(x => x)
                .First();
        }

        public static string GetOutputPath(string project, string configName = "Release|AnyCPU")
        {
            var xml = new XPathDocument(project);
            var nav = xml.CreateNavigator();

            var xPath =
                $@"/Project/PropertyGroup[@Condition=""'$(Configuration)|$(Platform)'=='{configName}'""]/OutputPath";
            var outputPathNode = nav.SelectSingleNode(xPath);
            return outputPathNode?.InnerXml;
        }

        private static IEnumerable<string> GetFrameworks(string project)
        {
            var xml = new XPathDocument(project);
            var nav = xml.CreateNavigator();
            var framework = nav.SelectSingleNode("/Project/PropertyGroup/TargetFramework");
            if (framework != null)
            {
                return new [] { framework.InnerXml };
            }

            var frameworks = nav.SelectSingleNode("/Project/PropertyGroup/TargetFrameworks");

            return frameworks.InnerXml.Split(";");
        }

        public static string GetProjectFilePath()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj").FirstOrDefault();
        }
    }
}