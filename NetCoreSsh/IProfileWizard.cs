namespace DotNetSsh
{
    public interface IProfileWizard
    {
        DeploymentProfile Configure(string profileName, ProjectMetadata projectPath, string username, DeploymentProfile existing = null);
    }
}