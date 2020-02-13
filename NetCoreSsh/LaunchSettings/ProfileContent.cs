namespace DotNetSsh.LaunchSettings
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.5.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ProfileContent
    {
        /// <summary>Identifies the debug target to run.</summary>
        [Newtonsoft.Json.JsonProperty("commandName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(int.MaxValue, MinimumLength = 1)]
        public string CommandName { get; set; } = "";

        /// <summary>The arguments to pass to the target being run.</summary>
        [Newtonsoft.Json.JsonProperty("commandLineArgs", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string CommandLineArgs { get; set; } = "";

        /// <summary>An absolute or relative path to the to the executable.</summary>
        [Newtonsoft.Json.JsonProperty("executablePath", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ExecutablePath { get; set; } = "";

        /// <summary>Sets the working directory of the command.</summary>
        [Newtonsoft.Json.JsonProperty("workingDirectory", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string WorkingDirectory { get; set; }

        /// <summary>Set to true if the browser should be launched.</summary>
        [Newtonsoft.Json.JsonProperty("launchBrowser", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? LaunchBrowser { get; set; } = false;

        /// <summary>The relative URL to launch in the browser.</summary>
        [Newtonsoft.Json.JsonProperty("launchUrl", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string LaunchUrl { get; set; }

        /// <summary>Set the environment variables as key/value pairs.</summary>
        [Newtonsoft.Json.JsonProperty("environmentVariables", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, string> EnvironmentVariables { get; set; }

        /// <summary>A semi-colon delimited list of URL(s) to configure for the web server.</summary>
        [Newtonsoft.Json.JsonProperty("applicationUrl", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ApplicationUrl { get; set; }

        /// <summary>Set to true to enable native code debugging.</summary>
        [Newtonsoft.Json.JsonProperty("nativeDebugging", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? NativeDebugging { get; set; } = false;

        /// <summary>Set to true to disable configuration of the site when running the Asp.Net Core Project profile.</summary>
        [Newtonsoft.Json.JsonProperty("externalUrlConfiguration", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? ExternalUrlConfiguration { get; set; } = false;

        /// <summary>Set to true to run the 64 bit version of IIS Express, false to run the x86 version.</summary>
        [Newtonsoft.Json.JsonProperty("use64Bit", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? Use64Bit { get; set; } = false;

        private System.Collections.Generic.IDictionary<string, object> additionalProperties = new System.Collections.Generic.Dictionary<string, object>();

        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return additionalProperties; }
            set { additionalProperties = value; }
        }


    }
}