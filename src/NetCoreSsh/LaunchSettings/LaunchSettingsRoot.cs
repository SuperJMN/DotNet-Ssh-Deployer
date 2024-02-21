using DotNetSsh.LaunchSettings.MyNamespace;

namespace DotNetSsh.LaunchSettings
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.5.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class LaunchSettingsRoot
    {
        /// <summary>A list of debug profiles</summary>
        [Newtonsoft.Json.JsonProperty("profiles", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, Profile> Profiles { get; set; }

        /// <summary>IIS and IIS Express settings</summary>
        [Newtonsoft.Json.JsonProperty("iisSettings", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IisSettings IisSettings { get; set; }

        private System.Collections.Generic.IDictionary<string, object> additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return additionalProperties; }
            set { additionalProperties = value; }
        }


    }
}