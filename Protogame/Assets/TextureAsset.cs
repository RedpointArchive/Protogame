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
    public class TextureAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public Texture2D Texture { get; private set; }
        public byte[] Data { get; private set; }

        public TextureAsset(IAssetContentManager assetContentManager, string name, byte[] data)
        {
            this.Name = name;
            this.Data = data;
            
            if (assetContentManager != null)
            {
                using (var stream = new MemoryStream(data))
                {
                    assetContentManager.SetStream(name, stream);
                    this.Texture = assetContentManager.Load<Texture2D>(name);
                    assetContentManager.UnsetStream(name);
                }
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(TextureAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to FontAsset.");
        }
    }
}

