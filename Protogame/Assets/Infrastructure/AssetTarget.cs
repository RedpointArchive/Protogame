namespace Protogame
{
    /// <summary>
    /// Indicates the target format for an asset while saving an asset.
    /// </summary>
    public enum AssetTarget
    {
        /// <summary>
        /// The asset should contain both source and compiled information, or more specifically,
        /// information should not be lost during the save as the resulting data will be used to
        /// load the asset again during this execution of the game.
        /// </summary>
        Runtime, 

        /// <summary>
        /// The asset should only contain source information, specifically that it should not
        /// contain any platform-specific data, and that a version of the asset can be
        /// compiled for any given platform from the information in the asset.
        /// </summary>
        SourceFile, 

        /// <summary>
        /// The asset should only contain compiled information.  This is used to indicate that
        /// the asset saver should generally return an instance of <see cref="CompiledAsset"/>
        /// if the asset requires platform-specific data.
        /// </summary>
        CompiledFile
    }
}