using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace DotNetSsh.Console
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var deployerApp = new DeployerApp();

            var result = Parser.Default.ParseArguments<AddVerbOptions, DeployVerbOptions>(args);

            var mapResult = await result
                .MapResult(
                    (AddVerbOptions o) =>
                    {
                        deployerApp.AddOrReplaceProfile(o);
                        return Task.FromResult(0);
                    }, async (DeployVerbOptions o) =>
                    {
                        await deployerApp.Deploy(o);
                        return 0;
                    }, errors =>
                    {
                        ShowHelp(result);
                        return Task.FromResult(1);
                    });

            return mapResult;
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
