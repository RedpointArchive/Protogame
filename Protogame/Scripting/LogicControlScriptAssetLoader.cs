namespace Protogame
{
    /// <summary>
    /// The asset loader for LogicControl scripts.
    /// </summary>
    public class LogicControlScriptAssetLoader : IAssetLoader
    {
        /// <summary>
        /// Returns whether this asset loader can handle the specified asset.
        /// </summary>
        /// <param name="data">
        /// The raw asset data to check.
        /// </param>
        /// <returns>
        /// Whether this asset loader can handle the specified asset.
        /// </returns>
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(LogicControlScriptAssetLoader).FullName;
        }

        /// <summary>
        /// Returns whether this asset loader can create a new instance of this asset.
        /// </summary>
        /// <returns>
        /// Whether this asset loader can create a new instance of this asset.
        /// </returns>
        public bool CanNew()
        {
            return false;
        }

        /// <summary>
        /// Returns the default representation for this asset.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        /// <param name="name">The asset name.</param>
        /// <returns>Always returns null.</returns>
        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        /// <summary>
        /// Constructs a new LogicControl script and returns it.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        /// <param name="name">The asset name.</param>
        /// <returns>Always returns null.</returns>
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return null;
        }

        /// <summary>
        /// Handles loading the specified LogicControl asset from it's raw data.
        /// </summary>
        /// <param name="assetManager">The asset manager.</param>
        /// <param name="name">The name of the LogicControl script.</param>
        /// <param name="data">The raw data associated with the asset.</param>
        /// <returns>The ScriptAsset from the load.</returns>
        public IAsset Handle(IAssetManager assetManager, string name, dynamic data)
        {
            return new ScriptAsset(
                name,
                (string)data.Code,
                new LogicControlScriptEngine((string)data.Code));
        }
    }
}