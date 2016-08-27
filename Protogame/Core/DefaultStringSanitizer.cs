// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IStringSanitizer"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IStringSanitizer</interface_ref>
    public class DefaultStringSanitizer : IStringSanitizer
    {
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