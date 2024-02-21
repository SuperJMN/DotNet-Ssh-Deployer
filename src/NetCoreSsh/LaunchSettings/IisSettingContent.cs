namespace DotNetSsh.LaunchSettings
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.5.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class IisSettingContent
    {
        /// <summary>Set to true to enable windows authentication for your site in IIS and IIS Express.</summary>
        [Newtonsoft.Json.JsonProperty("windowsAuthentication", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? WindowsAuthentication { get; set; } = false;

        /// <summary>Set to true to enable anonymous authentication for your site in IIS and IIS Express.</summary>
        [Newtonsoft.Json.JsonProperty("anonymousAuthentication", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? AnonymousAuthentication { get; set; } = true;

        /// <summary>Site settings to use with IISExpress profiles.</summary>
        [Newtonsoft.Json.JsonProperty("iisExpress", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IisExpress IisExpress { get; set; }

        private System.Collections.Generic.IDictionary<string, object> additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return additionalProperties; }
            set { additionalProperties = value; }
        }


    }
}