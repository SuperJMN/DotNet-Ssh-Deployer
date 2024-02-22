namespace DotNetSsh
{
    public class CustomizableSettings
    {
        public string DestinationPath { get; set; }
        public string Host { get; set; }
        public string AssemblyName { get; set; }
        public string Framework { get; set; }
        public string Display { get; set; }
        public string CommandAfterDeployment { get; set; }
        public bool RunAfterDeployment { get; set; }
        public bool CheckGitBeforeDeploy { get; set; }
        public Architecture Architecture { get; set; }
    }
}