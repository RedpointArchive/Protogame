using System;

namespace Protogame
{
    /// <summary>
    /// An image source that uses an RGBA array.
    /// </summary>
    /// <module>Image Processing</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IImageSource</interface_ref>
    public class ImageSourceFromRGBAArray : IImageSource
    {
        private readonly Func<byte[]> _getRgbaArray;

        private readonly Func<int> _getWidth; 

        public ImageSourceFromRGBAArray(Func<byte[]> getRGBAArray, Func<int> getWidth)
        {
            _getRgbaArray = getRGBAArray;
            _getWidth = getWidth;
        }

        public byte[] GetSourceAsBytes(out int width, out int height)
        {
            width = 0;
            height = 0;

            var arr = _getRgbaArray();
            if (arr == null)
            {
                return null;
            }
            
            width = _getWidth();
            height = (arr.Length/4)/width;
            return arr;
        }
    }
}