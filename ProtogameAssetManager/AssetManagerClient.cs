using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Dx.Runtime;
using NDesk.Options;
using Ninject;
using Protogame;

namespace ProtogameAssetManager
{
    public static class AssetManagerClient
    {
        /// <summary>
        /// Runs the asset manager side-by-side with another XNA program
        /// (for example the main game) and then rebinds the IoC providers
        /// for asset management so that assets can be changed in real-time.
        /// </summary>
        public static Process RunAndConnect(IKernel kernel, bool startProcess)
        {
            Process process = null;
            if (startProcess)
            {
                process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    Arguments = "--connect"
                };
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) =>
                {
                    Environment.Exit(1);
                };
                process.Start();
            }

            var factory = new DefaultDxFactory();
            var node = factory.CreateLocalNode(
                Caching.PushOnChange,
                Architecture.ServerClient);
            node.Network = new ProtogameAssetManagerNetwork(node, false);
            node.Join(ID.NewHash("asset manager"));

            var assetManagerProvider = new NetworkedAssetManagerProvider(node, kernel);
            kernel.Bind<IAssetManagerProvider>().ToMethod(x => assetManagerProvider);

            // Wait until the networked asset manager is ready.
            while (!assetManagerProvider.IsReady && (process == null || !process.HasExited))
                Thread.Sleep(100);

            return process;
        }
        
        /// <summary>
        /// This is a utility function that accepts the current command line
        /// arguments and parses them to determine whether or not the asset
        /// manager tool should be started along with the game.  If you need to
        /// parse additional arguments, you will need to perform the asset
        /// manager tool checks yourself.
        /// </summary>
        /// <param name="kernel">The kernel to bind the asset manager provider into.</param>
        /// <param name="args">The command line arguments provided to the program.</param>
        /// <typeparam name="T">The implementation of the asset manager provider if not starting the asset manager tool.</typeparam>
        public static void AcceptArgumentsAndSetup<T>(IKernel kernel, string[] args) where T : IAssetManagerProvider
        {
            var name = new FileInfo(Assembly.GetCallingAssembly().Location).Name;
            var startAssetManager = false;
            var listen = false;
            var options = new OptionSet
            {
                { "asset-manager", "Start the asset manager with the game.", v => startAssetManager = true },
                { "asset-manager-listen", "Start the game and wait for the asset manager to connect.", v => { startAssetManager = true; listen = true; } }
            };
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

            Process assetManagerProcess = null;
            if (startAssetManager)
            {
                assetManagerProcess = AssetManagerClient.RunAndConnect(kernel, !listen);
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
    }
}
