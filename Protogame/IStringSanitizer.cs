namespace Protogame
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An interface which provides mechanisms to sanitize text input.
    /// </summary>
    /// <module>Core API</module>
    public interface IStringSanitizer
    {
        /// <summary>
        /// Sanitizes the specified text such that it can be rendered with the specified font.
        /// </summary>
        /// <param name="font">The font that will render or measure the text.</param>
        /// <param name="text">The text to sanitize.</param>
        /// <returns>The text containing only characters that the font can render.</returns>
        string SanitizeCharacters(SpriteFont font, string text);
    }
}