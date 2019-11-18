namespace NetCoreSsh
{
    public class ProfileCreator
    {
        public DeploymentProfile Create(string name, string projectPath)
        {
            var options = new DeploymentOptionsBuilder()
                .FromProject(projectPath)
                .Destination.UseCredentials()
                .Build();

            return new DeploymentProfile(name, options);
        }
    }

}