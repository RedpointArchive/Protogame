namespace Protogame
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The EffectTexture interface.
    /// </summary>
    internal interface IEffectTexture
    {
        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        Texture2D Texture { get; set; }
    }
}