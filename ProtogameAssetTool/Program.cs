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
using ProtogameAssetTool32;

namespace ProtogameAssetTool
{
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
                    if (asset is FontAsset)
                    {
                        // This must be compiled under a 32-bit process.
                        var result = Stub32.Call(typeof(Program), "Force32Compile", output + "|" + platformName + "|" + asset.Name);
                        if (result == "pass")
                        {
                            Console.WriteLine("Compiled " + asset.Name + " for " + platform);
                        }
                        else
                        {
                            Console.WriteLine("Failed to compile " + asset.Name + " for " + platform);
                        }

                        continue;
                    }

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
        
        /// <summary>
        /// This forces 32-bit compilation of the specified asset.  Used to compile
        /// font assets which must be executed under a 32-bit process.
        /// </summary>
        /// <param name="arg">The | seperated list of arguments.</param>
        /// <returns>Whether the compilation succeeded.</returns>
        public static string Force32Compile(string arg)
        {
            var args = arg.Split('|');
            var output = args[0];
            var platformName = args[1];
            var assetName = args[2];

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

            // Perform compilation of the asset.
            var platform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), platformName);
            var outputPath = Path.Combine(output, platformName);

#if DEBUG
            var asset = ((LocalAsset)assetManager.GetUnresolved(assetName)).Instance;
#else
            var asset = assetManager.GetUnresolved(assetName);
#endif

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

                    return "pass";
                }
            }

            assetManager.Dirty(asset.Name);

            return "fail";
        }
    }
}
