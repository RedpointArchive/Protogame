//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class FontAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public SpriteFont Font { get; private set; }
        public string FontName { get; private set; }
        public int FontSize { get; private set; }
        public byte[] FontData { get; private set; }

        public FontAsset(IAssetContentManager assetContentManager, string name, string fontName, int fontSize, byte[] fontData)
        {
            this.Name = name;
            this.FontName = fontName;
            this.FontSize = fontSize;
            this.FontData = fontData;
            
            if (assetContentManager != null)
            {
                using (var stream = new MemoryStream(fontData))
                {
                    assetContentManager.SetStream(name, stream);
                    this.Font = assetContentManager.Load<SpriteFont>(name);
                    assetContentManager.UnsetStream(name);
                }
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(FontAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to FontAsset.");
        }
    }
}

