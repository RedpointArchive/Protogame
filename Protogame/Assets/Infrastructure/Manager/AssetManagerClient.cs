#if FALSE
using Dx.Runtime;
#endif

namespace Protogame
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using NDesk.Options;
    using Protoinject;

    /// <summary>
    /// The asset manager client.
    /// </summary>
    public static class AssetManagerClient
    {
        /// <summary>
        /// This is a utility function that accepts the current command line
        /// arguments and parses them to determine whether or not the asset
        /// manager tool should be started along with the game.  If you need to
        /// parse additional arguments, you will need to perform the asset
        /// manager tool checks yourself.
        /// </summary>
        /// <param name="kernel">
        /// The kernel to bind the asset manager provider into.
        /// </param>
        /// <param name="args">
        /// The command line arguments provided to the program.
        /// </param>
        /// <param name="extraOptions">
        /// The extra Options.
        /// </param>
        /// <typeparam name="T">
        /// The implementation of the asset manager provider if not starting the asset manager tool.
        /// </typeparam>
        public static void AcceptArgumentsAndSetup<T>(IKernel kernel, string[] args, params ExtraOption[] extraOptions)
            where T : IAssetManagerProvider
        {
            if (args == null)
            {
                args = new string[0];
            }

            var startAssetManager = false;
            var listen = false;

            if (args.Length > 0)
            {
                var name = new FileInfo(Assembly.GetCallingAssembly().Location).Name;
                var options = new OptionSet
                {
                    {"asset-manager", "Start the asset manager with the game.", v => startAssetManager = true},
                    {
                        "asset-manager-listen", "Start the game and wait for the asset manager to connect.", v =>
                        {
                            startAssetManager = true;
                            listen = true;
                        }
                    }
                };
                foreach (var e in extraOptions)
                {
                    options.Add(e.Prototype, e.Description, e.Action);
                }

                try
                {
                    options.Parse(args);
                }
                catch (OptionException ex)
                {
                    Console.Write(name + ": ");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Try `" + name + " --help` for more information.");
                    Environment.Exit(1);
                    return;
                }
            }

            Process assetManagerProcess = null;
            if (startAssetManager)
            {
                assetManagerProcess = RunAndConnect(kernel, !listen);
            }
            else
            {
                kernel.Bind<IAssetManagerProvider>().To<T>().InSingletonScope();
            }

            if (assetManagerProcess != null)
            {
                AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
                {
                    // Make sure we close down the asset manager process if it's there.
                    if (assetManagerProcess != null)
                    {
                        assetManagerProcess.Kill();
                    }
                };
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    // Make sure we close down the asset manager process if it's there.
                    if (assetManagerProcess != null)
                    {
                        assetManagerProcess.Kill();
                    }
                };
            }
        }

        /// <summary>
        /// Runs the asset manager side-by-side with another XNA program
        /// (for example the main game) and then rebinds the IoC providers
        /// for asset management so that assets can be changed in real-time.
        /// </summary>
        /// <param name="kernel">
        /// The kernel.
        /// </param>
        /// <param name="startProcess">
        /// The start Process.
        /// </param>
        /// <returns>
        /// The <see cref="Process"/>.
        /// </returns>
        public static Process RunAndConnect(IKernel kernel, bool startProcess)
        {
#if FALSE
            var node = new LocalNode();
            node.Bind(IPAddress.Loopback, 9838);
			#endif

            Process process = null;
            if (startProcess)
            {
                var assemblyPath = Assembly.GetExecutingAssembly().Location;
                var directory = new FileInfo(assemblyPath).Directory;
                var filename = Path.Combine(directory.FullName, "ProtogameAssetManager.exe");
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException(
                        "You must have ProtogameAssetManager.exe in " + "the same directory as your game.");
                }

                process = new Process();
                process.StartInfo = new ProcessStartInfo { FileName = filename, Arguments = "--connect" };
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => { Environment.Exit(1); };
                process.Start();
            }

#if FALSE
            var assetManagerProvider = new NetworkedAssetManagerProvider(node, kernel);
            kernel.Bind<IAssetManagerProvider>().ToMethod(x => assetManagerProvider);

            // Wait until the networked asset manager is ready.
            while (!assetManagerProvider.IsReady && (process == null || !process.HasExited))
                Thread.Sleep(100);
            #endif

            return process;
        }
    }

    /// <summary>
    /// The extra option.
    /// </summary>
    public struct ExtraOption
    {
        /// <summary>
        /// The action.
        /// </summary>
        public Action<string> Action;

        /// <summary>
        /// The description.
        /// </summary>
        public string Description;

        /// <summary>
        /// The prototype.
        /// </summary>
        public string Prototype;
    }
}