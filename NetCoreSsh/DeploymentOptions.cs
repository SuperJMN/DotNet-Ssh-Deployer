using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNetSsh
{
    public class DeploymentOptions
    {
        public string DestinationPath { get; set; }
        public Credentials Credentials { get; set; }
        public string Host { get; set; }
        public string AssemblyName { get; set; }
        public string Framework { get; set; }
        public string Display { get; set; }
        public bool RunAfterDeployment { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TargetDevice TargetDevice { get; set; }
    }
}