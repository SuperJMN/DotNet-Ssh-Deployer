using System.ComponentModel.DataAnnotations;

namespace DotNetSsh.LaunchSettings
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.5.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class IisBindingContent
    {
        /// <summary>The URL of the web site.</summary>
        [Newtonsoft.Json.JsonProperty("applicationUrl", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Uri ApplicationUrl { get; set; } = new System.Uri("");

        /// <summary>The SSL Port to use for the web site.</summary>
        [Newtonsoft.Json.JsonProperty("sslPort", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Range(0, 65535)]
        public int? SslPort { get; set; } = 0;

        private System.Collections.Generic.IDictionary<string, object> additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return additionalProperties; }
            set { additionalProperties = value; }
        }
    }
}