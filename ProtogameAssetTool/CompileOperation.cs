using Protogame;
using Protoinject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProtogameAssetTool
{
    public class CompileOperation : IOperation
    {
        public async Task Run(OperationArguments args)
        {
            foreach (var platform in args.Platforms)
            {
                Console.WriteLine("Starting compilation for " + platform + "...");

                var targetPlatform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), platform);

                var targetOutputPath = Path.Combine(new DirectoryInfo(args.OutputPath).FullName, platform);
                Directory.CreateDirectory(targetOutputPath);
                
                var kernel = new StandardKernel();
                kernel.Load<ProtogameAssetModule>();
                var sourceLayer = kernel.Get<LocalFilesystemAssetFsLayer>(new NamedConstructorArgument("basePath", Environment.CurrentDirectory));
                kernel.Unbind<IAssetFsLayer>();
                kernel.Bind<IAssetFsLayer>().ToMethod(ctx => sourceLayer);
                kernel.Bind<IAssetFsLayer>().ToMethod(ctx => ctx.Kernel.Get<LocalFilesystemAssetFsLayer>(new NamedConstructorArgument("basePath", targetOutputPath)));
                kernel.Unbind<ICompiledAssetFs>();
                kernel.Rebind<ICompiledAssetFs>().ToMethod(ctx => ctx.Kernel.Get<HostCompiledAssetFs>(new NamedConstructorArgument("targetPlatform", targetPlatform)));
                kernel.Rebind<IRenderBatcher>().To<NullRenderBatcher>();
                kernel.Unbind<IAssetCompiler>();
                (new ProtogameAssetModule()).LoadRawAssetStrategies(kernel);
                LoadAndBindTypes(kernel, args.Assemblies);

                var compiledAssetFs = kernel.Get<ICompiledAssetFs>();
                var assetFs = kernel.Get<IAssetFs>();

                Console.WriteLine("Scanning for content...");
                var compilationTasks = await compiledAssetFs.ListTasks().ConfigureAwait(false);
                var saveTasks = new List<Task>();
                var expectedPaths = new ConcurrentBag<string>();
                foreach (var compilationTask in compilationTasks)
                {
                    saveTasks.Add(compilationTask.ContinueWith(async compiledAssetTask =>
                    {
                        var compiledAsset = await compiledAssetTask.ConfigureAwait(false);
                        var originalAsset = await assetFs.Get(compiledAsset.Name).ConfigureAwait(false);
                        var targetPath = Path.Combine(targetOutputPath, compiledAsset.Name.Replace('.', Path.DirectorySeparatorChar)) + ".bin";
                        // Check that the source file exists on disk in order to add it to the expected paths.
                        if (await sourceLayer.Get(compiledAsset.Name).ConfigureAwait(false) != null)
                        {
                            expectedPaths.Add(targetPath);
                        }
                        if (originalAsset.ModificationTimeUtcTimestamp < compiledAsset.ModificationTimeUtcTimestamp)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                            using (var writer = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                Console.WriteLine("Saving " + compiledAsset.Name + " to disk for " + platform + "...");
                                using (var stream = await compiledAsset.GetContentStream().ConfigureAwait(false))
                                {
                                    await stream.CopyToAsync(writer).ConfigureAwait(false);
                                }
                            }
                        }
                    }));
                }

                await Task.WhenAll(saveTasks).ConfigureAwait(false);

                foreach (var task in saveTasks.Where(x => x.IsFaulted))
                {
                    Console.WriteLine(task.Exception);
                }

                Console.WriteLine("Deleting compiled assets that have no source asset...");

                var allActualCompiledAssets = FindAllCompiledAssets(targetOutputPath).Select(x => Path.Combine(targetOutputPath, x.Replace('.', Path.DirectorySeparatorChar) + ".bin").ToLowerInvariant()).ToArray();
                var allExpectedCompiledAssets = expectedPaths.Select(x => x.ToLowerInvariant()).ToArray();
                var hashsetWorking = new HashSet<string>(allActualCompiledAssets);
                var hashsetExpected = new HashSet<string>(allExpectedCompiledAssets);
                hashsetWorking.ExceptWith(hashsetExpected);
                foreach (var compiledAssetOnDisk in hashsetWorking)
                {
                    Console.WriteLine("Deleting " + compiledAssetOnDisk + "...");
                    File.Delete(compiledAssetOnDisk);
                }
            }
        }

        public static void LoadAndBindTypes(IKernel kernel, string[] assemblies)
        {
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
                            if (typeof(IAssetCompiler).IsAssignableFrom(type))
                            {
                                Console.WriteLine("Binding IAssetCompiler: " + type.Name);
                                kernel.Bind<IAssetCompiler>().To(type);
                            }
                            else if (type.GetInterfaces().Any(x => x.Name == "IAssetLoader`1"))
                            {
                                Console.WriteLine("Binding IAssetLoader<>: " + type.Name);
                                kernel.Bind(type.GetInterfaces().First(x => x.Name == "IAssetLoader`1")).To(type);
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
        }

        private IEnumerable<string> FindAllCompiledAssets(string outputRootPath, string prefix = "")
        {
            var dirInfo = new DirectoryInfo(outputRootPath);

            foreach (var file in dirInfo.GetFiles("*.bin"))
            {
                yield return prefix + file.Name.Substring(0, file.Name.Length - file.Extension.Length);
            }

            foreach (var dir in dirInfo.GetDirectories())
            {
                foreach (var file in this.FindAllCompiledAssets(dir.FullName, prefix + dir.Name + "."))
                {
                    yield return file;
                }
            }
        }
    }
}
