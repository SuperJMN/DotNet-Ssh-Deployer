namespace DotNetSsh
{
    public class DeploymentProfile
    {
        public string Name { get; }
        public CustomizableSettings Settings { get; }

        public DeploymentProfile(string name, CustomizableSettings settings)
        {
            Name = name;
            Settings = settings;
        }
    }
}