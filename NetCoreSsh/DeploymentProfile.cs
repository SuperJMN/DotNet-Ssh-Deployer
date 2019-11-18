namespace NetCoreSsh
{
    public class DeploymentProfile
    {
        public string Name { get; }
        public DeploymentOptions Options { get; }

        public DeploymentProfile(string name, DeploymentOptions options)
        {
            Name = name;
            Options = options;
        }
    }
}