using System.Reflection;
using System.Diagnostics;
using Process4;
using Protogame;
using System;
using System.Threading;
using Ninject;
using NDesk.Options;
using System.IO;

namespace ProtogameAssetManager
{
    public static class AssetManagerClient
    {
        /// <summary>
        /// Runs the asset manager side-by-side with another XNA program
        /// (for example the main game) and then rebinds the IoC providers
        /// for asset management so that assets can be changed in real-time.
        /// </summary>
        public static Process RunAndConnect(IKernel kernel)
        {
            var process = new Process();
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

            var node = new LocalNode(
                Assembly.GetExecutingAssembly(),
                new Process4.Attributes.DistributedAttribute(
                    Process4.Attributes.Architecture.ServerClient,
                    Process4.Attributes.Caching.PushOnChange));
            node.Network = new ProtogameAssetManagerNetwork(node, false);
            node.Join();

            var assetManagerProvider = new NetworkedAssetManagerProvider(node, kernel);
            kernel.Bind<IAssetManagerProvider>().ToMethod(x => assetManagerProvider);

            // Wait until the networked asset manager is ready.
            while (!assetManagerProvider.IsReady && !process.HasExited)
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
            var options = new OptionSet
            {
                { "asset-manager", "Start the asset manager with the game.", v => startAssetManager = true }
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
                return;
            }

            Process assetManagerProcess = null;
            if (startAssetManager)
            {
                assetManagerProcess = AssetManagerClient.RunAndConnect(kernel);
            }
            else
            {
                kernel.Bind<IAssetManagerProvider>().To<T>().InSingletonScope();
            }

            if (assetManagerProcess != null)
            {
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
