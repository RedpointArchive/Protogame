namespace Protogame
{
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// An implementation of <see cref="IStringSanitizer"/>.
    /// </summary>
    public class DefaultStringSanitizer : IStringSanitizer
    {
        /// <summary>
        /// Sanitizes the specified text such that it can be rendered with the specified font.
        /// </summary>
        /// <param name="font">The font that will render or measure the text.</param>
        /// <param name="text">The text to sanitize.</param>
        /// <returns>The text containing only characters that the font can render.</returns>
        public string SanitizeCharacters(SpriteFont font, string text)
        {
            if (font.Characters.Count == 0)
            {
                return string.Empty;
            }

            var defaultChar = font.DefaultCharacter ?? '?';
            if (!font.Characters.Contains(defaultChar))
            {
                defaultChar = font.Characters[0];
            }

            for (var i = 0; i < text.Length; i++)
            {
                if (!font.Characters.Contains(text[i]))
                {
                    text = text.Replace(text[i], defaultChar);
                }
            }

            return text;
        }
    }
}