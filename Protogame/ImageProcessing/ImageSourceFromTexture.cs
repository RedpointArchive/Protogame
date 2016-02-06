using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// An image source that uses an in-memory texture.
    /// </summary>
    /// <module>Image Processing</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IImageSource</interface_ref>
    public class ImageSourceFromTexture : IImageSource
    {
        private readonly Func<Texture2D> _getTexture2D;

        private byte[] _bytes;

        public ImageSourceFromTexture(Func<Texture2D> getTexture2D)
        {
            _getTexture2D = getTexture2D;
        }

        public byte[] GetSourceAsBytes(out int width, out int height)
        {
            width = 0;
            height = 0;

            var texture = _getTexture2D();
            if (texture == null)
            {
                return null;
            }

            var targetLength = texture.Width*texture.Height*4;
            if (_bytes == null || _bytes.Length != targetLength)
            {
                _bytes = new byte[targetLength];
            }

            texture.GetData(_bytes);
            width = texture.Width;
            height = texture.Height;

            return _bytes;
        }
    }
}