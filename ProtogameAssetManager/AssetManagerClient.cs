using System.Reflection;
using System.Diagnostics;
using Process4;
using Protogame;
using System;
using System.Threading;
using Ninject;

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

            var node = new LocalNode();
            node.Network = new ProtogameAssetManagerNetwork(node, false);
            node.Join();

            var assetManagerProvider = new NetworkedAssetManagerProvider(node);
            kernel.Bind<IAssetManagerProvider>().ToMethod(x => assetManagerProvider);

            // Wait until the networked asset manager is ready.
            while (!assetManagerProvider.IsReady && !process.HasExited)
                Thread.Sleep(100);

            return process;
        }
    }
}
