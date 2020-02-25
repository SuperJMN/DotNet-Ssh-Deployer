namespace DotNetSsh
{
    public class Deployment
    {
        public CustomizableSettings Settings { get; set; }
        public string ProjectPath { get; set; }
        public bool CleanDeploymentDestination { get; set; }
        public string BuildConfiguration { get; set; }
        public string Auth { get; set; }
        public DeploymentProfile Profile { get; set; }
        public AuthType AuthType { get; set; }
    }
}