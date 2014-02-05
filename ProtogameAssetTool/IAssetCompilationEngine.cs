namespace ProtogameAssetTool
{
    using System.Collections.Generic;

    /// <summary>
    /// The asset compilation engine interface, used for bulk compilation of assets.
    /// </summary>
    public interface IAssetCompilationEngine
    {
        /// <summary>
        /// Execute the engine, compiling the assets as needed.
        /// </summary>
        /// <param name="platforms">
        /// The target platforms for compilation.
        /// </param>
        /// <param name="output">
        /// The output directory.
        /// </param>
        void Execute(List<string> platforms, string output);
    }
}