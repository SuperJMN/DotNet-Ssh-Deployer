using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Renci.SshNet;
using Serilog;
using System.Diagnostics;
using DotNetSsh.UserSecrets;

namespace DotNetSsh
{
    public class Deployer : IDeployer
    {
        private readonly Func<ISecureSession> secureSession;

        public Deployer(Func<ISecureSession> secureSession)
        {
            this.secureSession = secureSession;
        }

        public async Task<Result> Deploy(DirectoryInfo source, Deployment settings, CredentialsManager credentialsManager)
        {
            try
            {
                Log.Information("Starting deployment...");

                using var clients = secureSession();
                await SyncFiles(source, settings, clients);
                GiveExecutablePermission(settings.Settings, clients);
                RunIfSelected(settings, clients.Ssh, credentialsManager);
            }
            catch (Exception e)
            {
                return Result.Failure($"Deployment failed: {e.Message}");
            }

            return Result.Success();
        }

        private static void RunIfSelected(Deployment settings, SshClient ssh, CredentialsManager credentialsManager)
        {
            if (!string.IsNullOrWhiteSpace(settings.Settings.CommandAfterDeployment))
            {
                if (settings.Settings.CommandAfterDeployment.Contains("sudo "))
                {
                    if (settings.AuthType == AuthType.PrivateKeyFile)
                    {
                        Log.Error("Sudo not supported with private key file!");
                    }
                    else
                    {
                        ShellStream shellStream = ssh.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
                        SwithToRoot(credentialsManager.Password, shellStream);
                        Log.Information($"Running post deployment command {settings.Settings.CommandAfterDeployment}");
                        ExecuteSudoCommand(settings.Settings.CommandAfterDeployment, shellStream);
                    }
                }
                else
                {
                    Log.Information($"Running post deployment command {settings.Settings.CommandAfterDeployment}");
                    var result = ssh.RunCommand(settings.Settings.CommandAfterDeployment);
                    if (!string.IsNullOrEmpty(result.Error))
                    {
                        Log.Error($"Command failed: {result.Error}");
                    }
                    else if (!string.IsNullOrEmpty(result.Result))
                    {
                        Log.Information($"Result: {result.Result}");
                    }
                }

                Log.Information($"Post deployment completed");
            }

            if (!settings.Settings.RunAfterDeployment)
            {
                return;
            }

            Log.Information($"Running application on display {settings.Settings.RunAfterDeployment}");
            var commandPath = GetExecutableName(settings.Settings);
            Log.Information("Application is running!");
            Log.Information("Waiting for the application to be closed...");
            Log.Warning("(this command will wait for the application to finish. Close it before trying to deploy again)");
            ssh.RunCommand($"DISPLAY={settings.Settings.Display} nohup {commandPath}");
        }

        private static void GiveExecutablePermission(CustomizableSettings settings, ISecureSession userAndPasswordSecureSession)
        {
            var executable = GetExecutableName(settings);
            userAndPasswordSecureSession.Ssh.RunCommand($"chmod +x {executable}");
        }

        private static string GetExecutableName(CustomizableSettings settings)
        {
            return settings.DestinationPath + "/" + settings.AssemblyName;
        }

        private static Task SyncFiles(DirectoryInfo source, Deployment settings,
            ISecureSession userAndPasswordSecureSession)
        {
            Log.Information("Deploying files...");

            PrepareTargetFolder(settings, userAndPasswordSecureSession);
            var synchronizer = new SshFolderSynchronizer(userAndPasswordSecureSession.Sftp);
            return synchronizer.Sync(source, settings.Settings.DestinationPath);
        }

        private static void PrepareTargetFolder(Deployment settings, ISecureSession userAndPasswordSecureSession)
        {
            if (settings.CleanDeploymentDestination && userAndPasswordSecureSession.Sftp.Exists(settings.Settings.DestinationPath))
            {
                Log.Verbose("The destination folder already exists. We are going to delete it.");
                userAndPasswordSecureSession.Ssh.DeleteExisting(settings.Settings.DestinationPath);
            }
        }

        private static void WriteStream(string cmd, ShellStream stream)
        {
            stream.WriteLine(cmd + "; echo this-is-the-end");
            while (stream.Length == 0)
                Thread.Sleep(500);
        }

        private static string ReadStream(ShellStream stream)
        {
            StringBuilder result = new StringBuilder();

            string line;
            while (!(line = stream.ReadLine()).EndsWith("\rthis-is-the-end"))
                result.AppendLine(line);

            return result.ToString();
        }

        private static void SwithToRoot(string password, ShellStream stream)
        {
            // Get logged in and get user prompt
            string prompt = stream.Expect(new Regex(@"[$>]"));
            //Console.WriteLine(prompt);

            // Send command and expect password or user prompt
            stream.WriteLine("sudo ls > null");
            prompt = stream.Expect(new Regex(@"([$#:])"));
            //Console.WriteLine(prompt);

            // Check to send password
            if (prompt.Contains(":"))
            {
                // Send password
                stream.WriteLine(password);
                prompt = stream.Expect(new Regex(@"[$#>]"));
                //Console.WriteLine(prompt);
            }
        }

        private static string ExecuteSudoCommand(string command, ShellStream stream)
        {
            WriteStream(command, stream);
            return ReadStream(stream);
        }
    }
}