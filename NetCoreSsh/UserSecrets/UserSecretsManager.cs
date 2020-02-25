using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetSsh.UserSecrets
{
    public class UserSecretsManager : IUserSecretsManager
    {
        private readonly string projectFile;

        public UserSecretsManager(string projectFile)
        {
            this.projectFile = projectFile;
        }

        public string Project => projectFile != null ? @$"--project ""{projectFile}""" : "";

        public void Set(string key, string value)
        {
            ProcessUtils.Run("dotnet", @$"user-secrets set ""{key}"" ""{value}"" {Project}");
        }

        public void Set(string value)
        {
            ProcessUtils.Run("cmd", $@"/c echo {value} | dotnet user-secrets set {Project}");
        }

        public List<string> List()
        {
            var result = ProcessUtils.Run("dotnet", @$"user-secrets list {Project}");
            return new List<string>(result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
        }

        public void Init()
        {
            ProcessUtils.Run("dotnet", @$"user-secrets init {Project}");
        }

        public void Clear()
        {
            ProcessUtils.Run("dotnet", @$"user-secrets clear {Project}");
        }
    }
}