namespace Protogame
{
    using System;

    /// <summary>
    /// The font asset saver.
    /// </summary>
    public class FontAssetSaver : IAssetSaver
    {
        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanHandle(IAsset asset)
        {
            return asset is FontAsset;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="dynamic"/>.
        /// </returns>
        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var fontAsset = asset as FontAsset;

            if (target == AssetTarget.CompiledFile)
            {
                if (fontAsset.PlatformData == null)
                {
                    throw new InvalidOperationException(
                        "Attempted save of font asset as a compiled file, but the font wasn't compiled.  This usually " +
                        "indicates that the font '" + fontAsset.FontName + "' is not installed on the current system.");
                }

                return new CompiledAsset
                {
                    Loader = typeof(FontAssetLoader).FullName, 
                    PlatformData = fontAsset.PlatformData
                };
            }

            return
                new
                {
                    Loader = typeof(FontAssetLoader).FullName, 
                    fontAsset.FontSize, 
                    fontAsset.FontName, 
                    fontAsset.UseKerning, 
                    fontAsset.Spacing, 
                    PlatformData = target == AssetTarget.SourceFile ? null : fontAsset.PlatformData
                };
        }
    }
}