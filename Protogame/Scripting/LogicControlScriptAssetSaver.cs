namespace Protogame
{
    /// <summary>
    /// The asset saver for LogicControl scripts.
    /// </summary>
    public class LogicControlScriptAssetSaver : IAssetSaver
    {
        /// <summary>
        /// Returns whether this asset saver can handle the specified asset.
        /// </summary>
        /// <param name="asset">
        /// The asset to check.
        /// </param>
        /// <returns>
        /// Whether this asset saver can handle the specified asset.
        /// </returns>
        public bool CanHandle(IAsset asset)
        {
            var scriptAsset = asset as ScriptAsset;

            return scriptAsset != null && scriptAsset.ScriptEngineType == typeof(LogicControlScriptEngine);
        }

        /// <summary>
        /// Handles saving the LogicControl script.
        /// </summary>
        /// <param name="asset">The script asset to save.</param>
        /// <param name="target">The saved asset target.</param>
        /// <returns>The raw asset data to be saved by <see cref="IRawAssetSaver"/>.</returns>
        public IRawAsset Handle(IAsset asset, AssetTarget target)
        {
            var scriptAsset = asset as ScriptAsset;

            return
                new AnonymousObjectBasedRawAsset(
                    new { Loader = typeof(LogicControlScriptAssetLoader).FullName, scriptAsset.Code });
        }
    }
}