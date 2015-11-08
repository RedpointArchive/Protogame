namespace ProtogameAssetTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Protoinject;
    using Protogame;

    /// <summary>
    /// The default implementation of the asset compilation engine interface.
    /// </summary>
    internal class DefaultAssetCompilationEngine : IAssetCompilationEngine
    {
        /// <summary>
        /// The dependency injection kernel.
        /// </summary>
        private readonly IKernel m_Kernel;

        /// <summary>
        /// The <see cref="IAssetOutOfDateCalculator"/>.
        /// </summary>
        private readonly IAssetOutOfDateCalculator m_AssetOutOfDateCalculator;

        /// <summary>
        /// The <see cref="IAssetCleanup"/>.
        /// </summary>
        private readonly IAssetCleanup m_AssetCleanup;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssetCompilationEngine"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The dependency injection kernel.
        /// </param>
        /// <param name="assetOutOfDateCalculator">
        /// The <see cref="IAssetOutOfDateCalculator"/>.
        /// </param>
        /// <param name="assetCleanup">
        /// The <see cref="IAssetCleanup"/>.
        /// </param>
        public DefaultAssetCompilationEngine(IKernel kernel, IAssetOutOfDateCalculator assetOutOfDateCalculator, IAssetCleanup assetCleanup)
        {
            this.m_Kernel = kernel;
            this.m_AssetOutOfDateCalculator = assetOutOfDateCalculator;
            this.m_AssetCleanup = assetCleanup;
        }

        /// <summary>
        /// Execute the engine, compiling the assets as needed.
        /// </summary>
        /// <param name="platforms">
        /// The target platforms for compilation.
        /// </param>
        /// <param name="output">
        /// The output directory.
        /// </param>
        public void Execute(List<string> platforms, string output)
        {
            // Set up the compiled asset saver.
            var compiledAssetSaver = new CompiledAssetSaver();

            // Retrieve the asset manager.
            var assetManager = this.m_Kernel.Get<LocalAssetManager>();
            assetManager.AllowSourceOnly = true;
            assetManager.SkipCompilation = true;

            // Retrieve the transparent asset compiler.
            var assetCompiler = this.m_Kernel.Get<ITransparentAssetCompiler>();

            // Retrieve all of the asset savers.
            var savers = this.m_Kernel.GetAll<IAssetSaver>();

            // For each of the platforms, perform the compilation of assets.
            foreach (var platformName in platforms)
            {
                Console.WriteLine("Starting compilation for " + platformName);
                var platform = (TargetPlatform)Enum.Parse(typeof(TargetPlatform), platformName);
                var outputPath = Path.Combine(output, platformName);
                assetManager.RescanAssets();

                // Create the output directory if it doesn't exist.
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                // Clean up any compiled assets that no longer have source files.
                this.m_AssetCleanup.Cleanup(outputPath);

                // Get a list of asset names that we need to recompile for this platform.
                var assetNames = this.m_AssetOutOfDateCalculator.GetAssetsForRecompilation(outputPath);

                foreach (var asset in assetNames.Select(assetManager.GetUnresolved))
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
                                    result is CompiledAsset);
                                Console.WriteLine("Compiled " + asset.Name + " for " + platform);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ERROR: Unable to compile " + asset.Name + " for " + platform);
                                Console.WriteLine("ERROR: " + ex.GetType().FullName + ": " + ex.Message);
                                break;
                            }
                        }
                    }

                    assetManager.Dirty(asset.Name);
                }
            }
        }
    }
}