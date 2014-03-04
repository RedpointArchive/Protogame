namespace Protogame
{
    /// <summary>
    /// The AssetGenerator interface, which allows assets to be generated based
    /// on other assets instead of source files.
    /// </summary>
    public interface IAssetGenerator
    {
        /// <summary>
        /// A list of asset names that this generator can provide if the requested
        /// asset is not present.
        /// </summary>
        string[] Provides();

        /// <summary>
        /// Generates the specified asset with the name that the user requested.
        /// </summary>
        /// <param name="assetManager">The asset manager requesting generation.</param>
        /// <param name="name">The name of the asset.</param>
        IAsset Generate(IAssetManager assetManager, string name);
    }
}