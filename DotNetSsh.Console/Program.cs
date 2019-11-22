using CommandLine;
using CommandLine.Text;

namespace DotNetSsh.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var deployerApp = new DeployerApp();

            var result = Parser.Default.ParseArguments<AddVerbOptions, DeployVerbOptions>(args);

            return result
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
                    }, errors =>
                    {
                        ShowHelp(result);
                        return 1;
                    });
        }

        private static void ShowHelp(ParserResult<object> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AddEnumValuesToHelpText = true;
                h.AddDashesToOption = true;
                return h;
            }, e => e, verbsIndex: true);

            System.Console.WriteLine(helpText);
        }
    }
}
