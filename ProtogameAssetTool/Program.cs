﻿namespace ProtogameAssetTool
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using Microsoft.Xna.Framework;
    using NDesk.Options;
    using Protoinject;
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
                { "m|operation=", "Specify the mode of operation (either 'bulk', 'remote' or 'builtin', default is 'bulk').", v => operation = v }
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

            if (IntPtr.Size != 8)
            {
                Console.Error.WriteLine("ERROR: Asset compilation is only supported on 64-bit machines.");
                Environment.Exit(1);
            }
            
            switch (operation)
            {
                case "remote":
                    RemoteCompileService();
                    break;
                case "builtin":
                    BuiltinCompile();
                    break;
                default:
                    BulkCompile(assemblies, platforms, output);
                    break;
            }
        }

        /// <summary>
        /// Compiles the built-in embedded resources.
        /// </summary>
        private static void BuiltinCompile()
        {
            // Create kernel.
            var kernel = new StandardKernel();
            kernel.Load<ProtogameAssetIoCModule>();
            kernel.Load<ProtogameScriptIoCModule>();
            var services = new GameServiceContainer();
            var assetContentManager = new AssetContentManager(services);
            kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);
            kernel.Bind<IRenderBatcher>().To<NullRenderBatcher>();

            // Only allow source and raw load strategies.
            kernel.Unbind<ILoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<LocalSourceLoadStrategy>();
            var assetModule = new ProtogameAssetIoCModule();
            assetModule.LoadRawAssetStrategies(kernel);

            // Set up remaining bindings.
            kernel.Bind<IAssetCleanup>().To<DefaultAssetCleanup>();
            kernel.Bind<IAssetOutOfDateCalculator>().To<DefaultAssetOutOfDateCalculator>();
            kernel.Bind<IAssetCompilationEngine>().To<DefaultAssetCompilationEngine>();

            // Rebind for builtin compilation.
            kernel.Rebind<IRawAssetLoader>().To<BuiltinRawAssetLoader>();

            // Set up the compiled asset saver.
            var compiledAssetSaver = new CompiledAssetSaver();

            // Retrieve the asset manager.
            var assetManager = kernel.Get<DefaultAssetManager>();
            assetManager.AllowSourceOnly = true;
            assetManager.SkipCompilation = true;

            // Retrieve the transparent asset compiler.
            var assetCompiler = kernel.Get<ITransparentAssetCompiler>();

            // Retrieve all of the asset savers.
            var savers = kernel.GetAll<IAssetSaver>();

            var rawLoader = kernel.Get<IRawAssetLoader>();

            // For each of the platforms, perform the compilation of assets.
            foreach (var platformName in new[]
                {
                    "Android",
                    "iOS",
                    "Linux",
                    "MacOSX",
                    "Ouya",
                    "Windows",
                })
            {
                Console.WriteLine("Starting compilation for " + platformName);
                var platform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), platformName);
                var outputPath = Environment.CurrentDirectory;

                // Create the output directory if it doesn't exist.
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                // Get a list of asset names that we need to recompile for this platform.
                var assetNames = rawLoader.ScanRawAssets();

                foreach (var asset in assetNames.Select(assetManager.GetUnresolved))
                {
                    Console.Write("Compiling " + asset.Name + " for " + platform + "... ");
                    try
                    {
                        assetCompiler.HandlePlatform(asset, platform, true);

                        foreach (var saver in savers)
                        {
                            var canSave = false;
                            try
                            {
                                canSave = saver.CanHandle(asset);
                            }
                            catch (Exception)
                            {
                            }

                            if (canSave)
                            {
                                try
                                {
                                    var result = saver.Handle(asset, AssetTarget.CompiledFile);
                                    compiledAssetSaver.SaveCompiledAsset(
                                        outputPath,
                                        asset.Name,
                                        result,
                                        result is CompiledAsset,
                                        platformName);
                                    Console.WriteLine("done.");
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("failed!");
                                    Console.WriteLine("ERROR: Unable to compile " + asset.Name + " for " + platform);
                                    Console.WriteLine("ERROR: " + ex.GetType().FullName + ": " + ex.Message);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("failed!");
                        Console.WriteLine("ERROR: Unable to compile " + asset.Name + " for " + platform);
                        Console.WriteLine("ERROR: " + ex.GetType().FullName + ": " + ex.Message);
                        break;
                    }
                }
            }
        }

        private static void BulkCompile(List<string> assemblies, List<string> platforms, string output)
        {
            // Create kernel.
            var kernel = new StandardKernel();
            kernel.Load<ProtogameAssetModule>();
            kernel.Load<ProtogameScriptIoCModule>();
            var services = new GameServiceContainer();
            var assetContentManager = new AssetContentManager(services);
            kernel.Bind<IAssetContentManager>().ToMethod(x => assetContentManager);
            kernel.Bind<IRenderBatcher>().To<NullRenderBatcher>();

            // Only allow source and raw load strategies.
            kernel.Unbind<ILoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<LocalSourceLoadStrategy>();
            var assetModule = new ProtogameAssetModule();
            assetModule.LoadRawAssetStrategies(kernel);

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
                            if (type.GetInterfaces().Any(x => x.Name == "IAssetCompiler`1"))
                            {
                                Console.WriteLine("Binding IAssetCompiler<>: " + type.Name);
                                kernel.Bind(type.GetInterfaces().First(x => x.Name == "IAssetCompiler`1")).To(type);
                            }
                            if (typeof(ILoadStrategy).IsAssignableFrom(type))
                            {
                                Console.WriteLine("Binding ILoadStrategy: " + type.Name);
                                kernel.Bind<ILoadStrategy>().To(type);
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
                            null,
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

                        try
                        {
                            response.ContentLength64 = effect.PlatformData.Data.Length;
                            response.OutputStream.Write(effect.PlatformData.Data, 0, effect.PlatformData.Data.Length);
                            response.Close();
                        }
                        catch (HttpListenerException)
                        {
                        }

                        break;
                    }
                    case "/compilefont":
                    {
                        var platform = (TargetPlatform) Convert.ToInt32(request.QueryString["platform"]);

                        var content = input.Split('\0');
                        var fontName = content[0];
                        var fontSize = int.Parse(content[1]);
                        var spacing = int.Parse(content[2]);
                        var useKerning = bool.Parse(content[3]);

                        var font = new FontAsset(
                            null,
                            "font",
                            fontName,
                            fontSize,
                            useKerning,
                            spacing,
                            null);

                        var compiler = new FontAssetCompiler();
                        try
                        {
                            compiler.Compile(font, platform);
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

                        try
                        {
                            response.ContentLength64 = font.PlatformData.Data.Length;
                            response.OutputStream.Write(font.PlatformData.Data, 0, font.PlatformData.Data.Length);
                            response.Close();
                        }
                        catch (HttpListenerException)
                        {
                        }

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
