using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class TextureAsset : IAsset, INativeAsset
    {
        private readonly IAssetContentManager _assetContentManager;
        private readonly byte[] _data;

        public TextureAsset(
            IAssetContentManager assetContentManager, 
            string name, 
            byte[] data)
        {
            _assetContentManager = assetContentManager;
            Name = name;
            _data = data;
        }
        
        public TextureAsset(Texture2D texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }

            Name = null;
            _data = null;
            Texture = texture;
            _assetContentManager = null;
        }
        
        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name of the asset.
        /// </value>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets the runtime, MonoGame texture.
        /// </summary>
        /// <value>
        /// The MonoGame texture.
        /// </value>
        public Texture2D Texture { get; private set; }
        
        public void ReadyOnGameThread()
        {
            if (_data == null)
            {
                return;
            }

            if (_assetContentManager == null)
            {
                throw new InvalidOperationException("Unable to reload dynamic texture.");
            }

            if (_assetContentManager != null && _data != null)
            {
                using (var stream = new MemoryStream(_data))
                {
                    _assetContentManager.SetStream(Name, stream);
                    _assetContentManager.Purge(Name);
                    try
                    {
                        var newTexture = _assetContentManager.Load<Texture2D>(Name);
                        if (newTexture != null)
                        {
                            Texture = newTexture;
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // This occurs when using assets without a game running, in which case we don't
                        // need the Texture2D anyway (since we are probably just after the compiled asset).
                        if (ex.Message != "No Graphics Device Service")
                        {
                            throw;
                        }
                    }

                    _assetContentManager.UnsetStream(Name);
                }
            }
        }
    }
}