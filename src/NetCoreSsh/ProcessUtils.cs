﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Serilog;

namespace DotNetSsh
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static class ProcessUtils
    {
        public static Result<string> Run(string command, string arguments)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,                                        
                }
            };

            Log.Verbose("Starting process {@Process}", new { process.StartInfo.FileName, process.StartInfo.Arguments });
            process.Start();
            Log.Verbose("Process started successfully");
            
            string output = process.StandardOutput.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(output))
            {
                Log.Verbose(output);
            }

            string err = process.StandardError.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(err))
            {
                Log.Error(err);
            }
            process.WaitForExit();

            Log.Verbose("Process output {Output}", output);

            var errorMessage = err != string.Empty ? err : output;
            return Result.FailureIf(() => process.ExitCode != 0, output, $"Process failed: {errorMessage}");
        }

        public static string RunSilently(string command, string arguments)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,
                    CreateNoWindow = true,                                        
                }
            };

            Log.Verbose("Starting process {@Process}", new { process.StartInfo.FileName, process.StartInfo.Arguments });
            process.Start();
            Log.Verbose("Process started successfully");
            
            string output = process.StandardOutput.ReadToEnd();
           
            process.WaitForExit();
            Log.Verbose("Process output {Output}", output);
            return output;
        }


        public static async Task<int> RunProcessAsync(string fileName, string args = "",
            IObserver<string> outputObserver = null, IObserver<string> errorObserver = null)
        {
            // ReSharper disable once ConvertToUsingDeclaration
            using (var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            })
            {
                return await RunProcessAsync(process, outputObserver, errorObserver).ConfigureAwait(false);
            }
        }

        private static Task<int> RunProcessAsync(Process process, IObserver<string> outputObserver,
            IObserver<string> errorObserver)
        {
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (s, ea) => tcs.SetResult(process.ExitCode);

            if (outputObserver != null)
            {
                process.OutputDataReceived += (s, ea) => outputObserver.OnNext(ea.Data);
            }

            if (errorObserver != null)
            {
                process.ErrorDataReceived += (s, ea) => errorObserver.OnNext(ea.Data);
            }

            Log.Verbose("Starting process {@Process}", new { process.StartInfo.FileName, process.StartInfo.Arguments });
            bool started = process.Start();
            Log.Verbose("Process started successfully");

            if (!started)
            {
                //you may allow for the process to be re-used (started = false) 
                //but I'm not sure about the guarantees of the Exited event in such a case
                throw new InvalidOperationException("Could not start process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}