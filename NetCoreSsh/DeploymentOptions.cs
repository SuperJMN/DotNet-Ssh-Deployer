namespace DotNetSsh
{
    public class DeploymentOptions
    {
        public string Project { get; set; }
        public bool CleanDeploymentDestination { get; set; }
        public string Configuration { get; set; }
        public AuthType AuthType { get; set; }
        public string Auth { get; set; }
        public string Profile { get; set; }
        public bool Verbose { get; set; }
    }
}