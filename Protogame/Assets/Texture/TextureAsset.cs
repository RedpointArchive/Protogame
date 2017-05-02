using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class TextureAsset : IAsset, INativeAsset, IDisposable
    {
        private readonly IAssetContentManager _assetContentManager;
        private byte[] _data;

        public TextureAsset(
            IAssetContentManager assetContentManager, 
            string name, 
            byte[] data,
            int originalWidth,
            int originalHeight)
        {
            _assetContentManager = assetContentManager;
            Name = name;
            _data = data;
            OriginalWidth = originalWidth;
            OriginalHeight = originalHeight;
            OriginalSize = new Point(originalWidth, originalHeight);
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
            OriginalWidth = texture.Width;
            OriginalHeight = texture.Height;
            OriginalSize = new Point(texture.Width, texture.Height);
        }
        
        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name of the asset.
        /// </value>
        public string Name { get; }
        
        /// <summary>
        /// Gets the runtime, MonoGame texture.
        /// </summary>
        /// <value>
        /// The MonoGame texture.
        /// </value>
        public Texture2D Texture { get; private set; }
        
        /// <summary>
        /// The original width of the texture, before it was compiled.  When textures are compiled in Protogame,
        /// they are converted to a square, power-of-two texture by default.
        /// </summary>
        public int OriginalWidth { get; }

        /// <summary>
        /// The original height of the texture, before it was compiled.  When textures are compiled in Protogame,
        /// they are converted to a square, power-of-two texture by default.
        /// </summary>
        public int OriginalHeight { get; }

        /// <summary>
        /// The original size of the texture, before it was compiled.  When textures are compiled in Protogame,
        /// they are converted to a square, power-of-two texture by default.
        /// </summary>
        public Point OriginalSize { get; }

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

                // Free the resource from main memory since it is now loaded into the GPU.  If the
                // resource is ever removed from the GPU (i.e. UnloadContent occurs followed by
                // LoadContent), then the asset will need to be reloaded through the asset management
                // system.
                _data = null;
            }
        }

        public void Dispose()
        {
            Texture?.Dispose();
        }
    }
}