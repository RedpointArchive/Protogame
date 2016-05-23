namespace Protogame
{
    /// <summary>
    /// Defines the format of level data in level assets.
    /// </summary>
    /// <module>Assets</module>
    public enum LevelDataFormat
    {
        /// <summary>
        /// The level data is in an unknown format.
        /// </summary>
        Unknown,

        /// <summary>
        /// The level data is exported from Ogmo Editor.
        /// </summary>
        OgmoEditor,

        /// <summary>
        /// The level data is exported from an ATF-based level editor.
        /// </summary>
        ATF,
    }
}
