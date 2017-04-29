using Protogame;
using Protoinject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProtogameAssetTool
{
    public class BuiltinOperation : IOperation
    {
        private class HostCompiledAssetFs : CompiledAssetFs
        {
            public HostCompiledAssetFs(IAssetFs assetFs, IAssetCompiler[] compilers, TargetPlatform targetPlatform) : base(assetFs, compilers, targetPlatform)
            {
            }

            protected override void OnCompileStart(IAssetFsFile assetFile, TargetPlatform targetPlatform)
            {
                Console.WriteLine("Compiling " + assetFile.Name + " for " + targetPlatform.ToString() + "...");
            }

            protected override void OnCompilerMissing(IAssetFsFile assetFile, TargetPlatform targetPlatform)
            {
                Console.WriteLine("No compiler available for " + assetFile.Name);
            }

            protected override void OnCompileFinish(IAssetFsFile assetFile, IAssetFsFile compiledAssetFile, TargetPlatform targetPlatform)
            {
                Console.WriteLine("Compiled " + assetFile.Name + " for " + targetPlatform.ToString());
            }
        }

        private class SourceAssetFsLayer : LocalFilesystemAssetFsLayer
        {
            public SourceAssetFsLayer(string basePath) : base(basePath)
            {
            }

            protected override bool AcceptAsset(string assetName, string fullPath)
            { 
                // Exclude compiled assets from our source layer.
                if (fullPath.EndsWith(".bin"))
                {
                    return false;
                }

                return base.AcceptAsset(assetName, fullPath);
            }
        }

        public async Task Run(OperationArguments args)
        {
            var platforms = new[]
            {
                "Android",
                "iOS",
                "Linux",
                "MacOSX",
                "Windows"
            };

            foreach (var platform in platforms)
            {
                Console.WriteLine("Starting compilation for " + platform + "...");

                var targetPlatform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), platform);

                var targetOutputPath = Environment.CurrentDirectory;
                
                var kernel = new StandardKernel();
                kernel.Load<ProtogameAssetModule>();
                var sourceLayer = kernel.Get<SourceAssetFsLayer>(new NamedConstructorArgument("basePath", Environment.CurrentDirectory));
                kernel.Unbind<IAssetFsLayer>();
                kernel.Bind<IAssetFsLayer>().ToMethod(ctx => sourceLayer);
                kernel.Unbind<ICompiledAssetFs>();
                kernel.Rebind<ICompiledAssetFs>().ToMethod(ctx => ctx.Kernel.Get<HostCompiledAssetFs>(new NamedConstructorArgument("targetPlatform", targetPlatform)));
                kernel.Rebind<IRenderBatcher>().To<NullRenderBatcher>();
                kernel.Unbind<IAssetCompiler>();
                (new ProtogameAssetModule()).LoadRawAssetStrategies(kernel);

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
                        //var originalAsset = await assetFs.Get(compiledAsset.Name).ConfigureAwait(false);
                        var targetPath = Path.Combine(targetOutputPath, compiledAsset.Name) + "-" + platform + ".bin";
                        // Check that the source file exists on disk in order to add it to the expected paths.
                        if (await sourceLayer.Get(compiledAsset.Name).ConfigureAwait(false) != null)
                        {
                            expectedPaths.Add(targetPath);
                        }
                        //if (originalAsset.ModificationTimeUtcTimestamp < compiledAsset.ModificationTimeUtcTimestamp)
                        //{
                            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                            using (var writer = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                Console.WriteLine("Saving " + compiledAsset.Name + " to disk for " + platform + "...");
                                using (var stream = await compiledAsset.GetContentStream().ConfigureAwait(false))
                                {
                                    await stream.CopyToAsync(writer).ConfigureAwait(false);
                                }
                            }
                        //}
                    }));
                }

                await Task.WhenAll(saveTasks).ConfigureAwait(false);

                Console.WriteLine("Deleting compiled assets that have no source asset...");

                var allActualCompiledAssets = FindAllCompiledAssets(targetOutputPath).Where(x => x.EndsWith("-" + platform)).Select(x => Path.Combine(targetOutputPath, x + ".bin").ToLowerInvariant()).ToArray();
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
