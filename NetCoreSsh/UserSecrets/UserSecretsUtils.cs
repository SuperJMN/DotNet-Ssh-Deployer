using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetSsh.UserSecrets
{
    public static class UserSecretsUtils
    {
        private const string SecretsKey = "SshDeployer";

        public static IList<Profile> FromSecrets(this IUserSecretsManager userSecretsManager)
        {
            var secrets = userSecretsManager.List();

            if (!secrets.Any())
            {
                return new List<Profile>();
            }

            var deployerSecrets = secrets.Where(x => x.Contains(SecretsKey)).Select(ToPlainJsonEntry);

            var joined = string.Join(",", deployerSecrets);
            var obj = JObject.Parse("{" + joined + "}");
            var dict = obj.ToObject<Dictionary<string, string>>();
            var unflattened = JsonFlattener.Unflatten(dict);

            if (unflattened == null)
            {
                return new List<Profile>();
            }

            var secret = unflattened.ToObject<Root>();
            return secret.Profiles;
        }

        public static void ToSecrets(this IUserSecretsManager userSecretsManager, IList<Profile> secrets)
        {
            var serialized = JsonConvert.SerializeObject(new Root()
            {
                Profiles = secrets,
            });

            userSecretsManager.Set(serialized);
        }
        
        private static string ToPlainJsonEntry(string secretEntryString)
        {
            return string.Join(':', secretEntryString.Split("=", 2).Select(str => @$"""{str.Trim()}"""));
        }
    }
}