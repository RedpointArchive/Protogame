using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using NDesk.Options;
using Newtonsoft.Json;
using Ninject;
using Protogame;

namespace ProtogameAssetTool
{
    using System.Diagnostics;

    using Ninject.Planning.Targets;

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

            // Load additional assemblies.
            foreach (var filename in assemblies)
            {
                var file = new FileInfo(filename);
                try
                {
                    var assembly = Assembly.LoadFile(file.FullName);
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

            // Set up the compiled asset saver.
            var compiledAssetSaver = new CompiledAssetSaver();

            // Retrieve the asset manager.
            var assetManager = kernel.Get<LocalAssetManager>();
            assetManager.AllowSourceOnly = true;
            assetManager.SkipCompilation = true;

            // Retrieve the transparent asset compiler.
            var assetCompiler = kernel.Get<ITransparentAssetCompiler>();

            // Retrieve all of the asset savers.
            var savers = kernel.GetAll<IAssetSaver>();

            // For each of the platforms, perform the compilation of assets.
            foreach (var platformName in platforms)
            {
                Console.WriteLine("Starting compilation for " + platformName);
                var platform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), platformName);
                var outputPath = Path.Combine(output, platformName);

                foreach (var asset in assetManager.GetAll())
                {
                    var compiledAsset = assetCompiler.HandlePlatform(asset, platform);

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
                            var result = saver.Handle(asset, AssetTarget.CompiledFile);
                            compiledAssetSaver.SaveCompiledAsset(outputPath, asset.Name, result, result is CompiledAsset);
                            Console.WriteLine("Compiled " + asset.Name + " for " + platform);
                            break;
                        }
                    }

                    assetManager.Dirty(asset.Name);
                }
            }
        }
    }
}
