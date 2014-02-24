namespace ProtogameAssetTool
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using Assimp.Configs;
    using Microsoft.Xna.Framework;
    using NDesk.Options;
    using Ninject;
    using Protogame;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var assemblies = new List<string>();
            var platforms = new List<string>();
            var output = string.Empty;
            var operation = string.Empty;

            var options = new OptionSet
            {
                { "a|assembly=", "Load an assembly.", v => assemblies.Add(v) },
                { "p|platform=", "Specify one or more platforms to target.", v => platforms.Add(v) },
                { "o|output=", "Specify the output folder for the compiled assets.", v => output = v },
                { "m|operation=", "Specify the mode of operation (either 'bulk' or 'remote', default is 'bulk').", v => operation = v }
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

            // Deploy the correct MojoShader DLL.
            MojoShaderDeploy.Deploy();

            switch (operation)
            {
                case "remote":
                    RemoteCompileService();
                    break;
                default:
                    BulkCompile(assemblies, platforms, output);
                    break;
            }
        }

        private static void BulkCompile(List<string> assemblies, List<string> platforms, string output)
        {
            // Create kernel.
            var kernel = new StandardKernel();
            kernel.Load<ProtogameAssetIoCModule>();
            var services = new GameServiceContainer();
            var assetContentManager = new AssetContentManager(services);
            kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);

            // Only allow source and raw load strategies.
            kernel.Unbind<ILoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<LocalSourceLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawTextureLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawEffectLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawModelLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawAudioLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawLevelLoadStrategy>();

            // The assembly load strategy is required for references.
            // Assets loaded with the assembly load strategy won't have
            // any savers defined, so they won't ever get processed.
            kernel.Bind<ILoadStrategy>().To<AssemblyLoadStrategy>();

            // Load additional assemblies.
            foreach (var filename in assemblies)
            {
                var file = new FileInfo(filename);
                try
                {
                    var assembly = Assembly.LoadFrom(file.FullName);
                    foreach (var type in assembly.GetTypes())
                    {
                        try
                        {
                            if (type.IsAbstract || type.IsInterface)
                                continue;
                            if (type.Assembly == typeof(FontAsset).Assembly)
                                continue;
                            if (typeof(IAssetLoader).IsAssignableFrom(type))
                            {
                                Console.WriteLine("Binding IAssetLoader: " + type.Name);
                                kernel.Bind<IAssetLoader>().To(type);
                            }
                            if (typeof(IAssetSaver).IsAssignableFrom(type))
                            {
                                Console.WriteLine("Binding IAssetSaver: " + type.Name);
                                kernel.Bind<IAssetSaver>().To(type);
                            }
                        }
                        catch
                        {
                            // Might not be able to load the assembly, so just skip over it.
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Can't load " + file.Name);
                }
            }

            // Set up remaining bindings.
            kernel.Bind<IAssetCleanup>().To<DefaultAssetCleanup>();
            kernel.Bind<IAssetOutOfDateCalculator>().To<DefaultAssetOutOfDateCalculator>();
            kernel.Bind<IAssetCompilationEngine>().To<DefaultAssetCompilationEngine>();

            // Get the asset compilation engine.
            var compilationEngine = kernel.Get<IAssetCompilationEngine>();
            compilationEngine.Execute(platforms, output);
        }

#if PLATFORM_WINDOWS
        private static void RemoteCompileService()
        {
            var udpClient = new UdpClient(4321);
            var thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        IPEndPoint endpoint = null;
                        var bytes = udpClient.Receive(ref endpoint);
                        var message = Encoding.ASCII.GetString(bytes);

                        if (message == "request compiler")
                        {
                            var result = Encoding.ASCII.GetBytes("provide compiler");
                            udpClient.Send(result, result.Length, new IPEndPoint(endpoint.Address, 4321));
                            Console.WriteLine("Providing compiler services for " + endpoint.Address);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();

            var server = new HttpListener();
            server.Prefixes.Add("http://+:8080/");
            try
            {
                server.Start();
            }
            catch (HttpListenerException ex)
            {
                var args = "http add urlacl http://+:8080/ user=Everyone listen=yes";

                var psi = new ProcessStartInfo("netsh", args);
                psi.Verb = "runas";
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = true;

                Process.Start(psi).WaitForExit();

                args = "advfirewall firewall add rule name=\"Port 8080 for Protogame Remote Compiler\" dir=in action=allow protocol=TCP localport=8080";

                psi = new ProcessStartInfo("netsh", args);
                psi.Verb = "runas";
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = true;

                Process.Start(psi).WaitForExit();

                server = new HttpListener();
                server.Prefixes.Add("http://+:8080/");
                server.Start();
            }

            Console.WriteLine("Remote compiler for Protogame started on port 4321 (UDP) and port 8080 (TCP)");

            while (true)
            {
                var context = server.GetContext();
                var request = context.Request;
                var response = context.Response;

                Console.WriteLine("Request: " + request.RawUrl);

                string input = null;
                using (var reader = new StreamReader(request.InputStream))
                {
                    input = reader.ReadToEnd();
                }

                switch (request.Url.AbsolutePath)
                {
                    case "/compileeffect":
                    {
                        var platform = (TargetPlatform) Convert.ToInt32(request.QueryString["platform"]);

                        var effect = new EffectAsset(
                            null,
                            "effect",
                            input,
                            null,
                            true);

                        var compiler = new EffectAssetCompiler();
                        try
                        {
                            compiler.Compile(effect, platform);
                        }
                        catch (Exception ex)
                        {
                            response.StatusCode = 500;
                            var result = Encoding.ASCII.GetBytes(ex.Message);
                            response.ContentLength64 = result.Length;
                            response.OutputStream.Write(result, 0, result.Length);
                            response.Close();
                            break;
                        }

                        response.ContentLength64 = effect.PlatformData.Data.Length;
                        response.OutputStream.Write(effect.PlatformData.Data, 0, effect.PlatformData.Data.Length);
                        response.Close();
                        break;
                    }
                    default:
                    {
                        response.StatusCode = 404;
                        var result = Encoding.ASCII.GetBytes("not found");
                        response.ContentLength64 = result.Length;
                        response.OutputStream.Write(result, 0, result.Length);
                        response.Close();
                        break;
                    }
                }
            }
        }
#else
        private static void RemoteCompileService()
        {
            Console.WriteLine("Protogame Remote Compiler can only be run under Windows");
            return;
        }
#endif
    }
}
