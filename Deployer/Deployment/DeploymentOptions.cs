namespace NetCoreSsh.Deployment
{
    internal class DeploymentOptions
    {
        public string DestinationPath { get; set; }
        public Credentials Credentials { get; set; }
        public string Host { get; set; }
        public string AssemblyName { get; set; }
        public string PublishFolder { get; set; }
        public string Project { get; set; }
        public string Framework { get; set; }
        public string Display { get; set; }
        public bool RunAfterDeployment { get; set; }
        public string Runtime { get; set; }
    }
}