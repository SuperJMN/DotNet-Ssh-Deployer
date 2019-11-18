using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace DotNetSsh.Console
{
    class Program
    {
        private const string ProfileStoreFilename = "deployment.json";

        static int Main(string[] args)
        {
            var deployerApp = new DeployerApp();

            return Parser.Default.ParseArguments<AddVerbOptions, DeployVerbOptions>(args)
                .MapResult(
                    (AddVerbOptions o) =>
                    {
                        deployerApp.AddOrReplaceProfile(o);
                        return 0;
                    }, 
                    (DeployVerbOptions o) =>
                    {
                        deployerApp.Deploy(o);
                        return 0;
                    }, errors => 1);
        }

        private static void SetupLogging(bool isVerbose)
        {
            var loggingLevelSwitch = new LoggingLevelSwitch();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                .CreateLogger();

            loggingLevelSwitch.MinimumLevel = isVerbose ? LogEventLevel.Verbose : LogEventLevel.Information;
        }
    }
}
