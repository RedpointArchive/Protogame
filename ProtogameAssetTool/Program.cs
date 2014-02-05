namespace ProtogameAssetTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
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

            var options = new OptionSet
            {
                { "a|assembly=", "Load an assembly.", v => assemblies.Add(v) },
                { "p|platform=", "Specify one or more platforms to target.", v => platforms.Add(v) },
                { "o|output=", "Specify the output folder for the compiled assets.", v => output = v }
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
    }
}
