using System;
using System.Collections.Generic;
using NDesk.Options;
using System.Threading.Tasks;

namespace ProtogameAssetTool
{    
    public static class Program
    {
        public static void Main(string[] args)
        {
            var assemblies = new List<string>();
            var platforms = new List<string>();
            var output = string.Empty;
            var operation = string.Empty;
            string androidPlatformTools = null;

            var options = new OptionSet
            {
                { "a|assembly=", "Load an assembly.", v => assemblies.Add(v) },
                { "p|platform=", "Specify one or more platforms to target.", v => platforms.Add(v) },
                { "o|output=", "Specify the output folder for the compiled assets.", v => output = v },
                { "m|operation=", "Specify the mode of operation (either 'compile', 'server' or 'builtin', default is 'compile').", v => operation = v },
                { "android-platform-tools=", "Specifies the path to ADB (used for connecting to a game running on an Android device)", v => androidPlatformTools = v },
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.Write("ProtogameAssetTool.exe: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try `ProtogameAssetTool.exe --help` for more information.");
                Environment.Exit(1);
                return;
            }

            var operationArguments = new OperationArguments
            {
                Assemblies = assemblies.ToArray(),
                Platforms = platforms.ToArray(),
                AndroidPlatformTools = androidPlatformTools,
                OutputPath = output
            };

            IOperation operationInst;
            switch (operation)
            {
                case "remote":
                    throw new NotSupportedException();
                case "server":
                    operationInst = new ServerOperation();
                    break;
                case "builtin":
                    operationInst = new BuiltinOperation();
                    break;
                case "compile":
                default:
                    operationInst = new CompileOperation();
                    break;
            }

            var task = Task.Run(async () => await operationInst.Run(operationArguments).ConfigureAwait(false));
            task.Wait();
        }
    }
}
