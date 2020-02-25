using System.Collections.Generic;

namespace DotNetSsh.UserSecrets
{
    public interface IUserSecretsManager
    {
        void Set(string key, string value);
        List<string> List();
        void Init();
        void Clear();
        void Set(string value);
    }
}