using DotNetSsh.LaunchSettings.MyNamespace;

namespace DotNetSsh
{
    public interface ILaunchSettingsProfileRepository
    {
        Profile Get(string name);
        void AddOrUpdate(string name, Profile profile);
    }
}