using System;
using System.Reflection;
using NetCoreSsh;

namespace ConsoleApp1
{
    public class ProfileCreator
    {
        public DeploymentProfile Create(string name, string projectPath, TargetDevice device)
        {
            var options = new DeploymentOptionsBuilder()
                .FromProject(projectPath)
                .ForDevice(device)
                .Destination.UseCredentials()
                .Build();

            return new DeploymentProfile(name, options);
        }
    }

}