using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A texture asset.
    /// <para>
    /// This represents a texture, stored in a Protogame asset.
    /// </para>
    /// </summary>
    /// <module>Assets</module>
    public class TextureAsset : MarshalByRefObject, IAsset, INativeAsset
    {
        /// <summary>
        /// The m_ asset content manager.
        /// </summary>
        private readonly IAssetContentManager m_AssetContentManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureAsset"/> class.
        /// <para>
        /// This constructor is called by <see cref="TextureAssetLoader"/> when loading a texture asset.
        /// </para>
        /// </summary>
        /// <param name="assetContentManager">
        /// The asset content manager used to create the MonoGame asset from the compiled data.
        /// </param>
        /// <param name="name">
        /// The name of the texture asset.
        /// </param>
        /// <param name="rawData">
        /// The raw data used for compilation, or null if there is no source information.
        /// </param>
        /// <param name="data">
        /// The platform-specific data used at runtime, or null if there is no compiled information.
        /// </param>
        /// <param name="sourcedFromRaw">
        /// Whether or not this asset was sourced from a purely raw asset file (such as a PNG).
        /// </param>
        public TextureAsset(
            IAssetContentManager assetContentManager, 
            string name, 
            byte[] rawData, 
            PlatformData data, 
            bool sourcedFromRaw)
        {
            this.Name = name;
            this.RawData = rawData;
            this.PlatformData = data;
            this.m_AssetContentManager = assetContentManager;
            this.SourcedFromRaw = sourcedFromRaw;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureAsset"/> class.
        /// <para>
        /// Create a new TextureAsset from a in-memory texture.  Used when you need to pass in a
        /// generated texture into a function that accepts a TextureAsset.  TextureAssets created
        /// with this constructor can not be saved or baked by an asset manager.
        /// </para>
        /// </summary>
        /// <param name="texture">
        /// The MonoGame texture to assign to this texture asset.
        /// </param>
        public TextureAsset(Texture2D texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }

            this.Name = null;
            this.Texture = texture;
            this.RawData = null;
            this.PlatformData = null;
            this.m_AssetContentManager = null;
            this.SourcedFromRaw = true;
        }

        /// <summary>
        /// Gets a value indicating whether the asset only contains compiled information.
        /// </summary>
        /// <value>
        /// Whether the asset only contains compiled information.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return this.RawData == null;
            }
        }

        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name of the asset.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the platform-specific data associated with this asset.
        /// </summary>
        /// <seealso cref="PlatformData"/>
        /// <value>
        /// The platform-specific data for this asset.
        /// </value>
        public PlatformData PlatformData { get; set; }

        /// <summary>
        /// Gets or sets the raw texture data.  This is the source information used to compile the asset.
        /// </summary>
        /// <value>
        /// The raw texture data.
        /// </value>
        public byte[] RawData { get; set; }

        /// <summary>
        /// Gets a value indicating whether the asset only contains source information.
        /// </summary>
        /// <value>
        /// Whether the asset only contains source information.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this asset was sourced from a raw file (such as a PNG image).
        /// </summary>
        /// <value>
        /// The sourced from raw.
        /// </value>
        public bool SourcedFromRaw { get; private set; }

        /// <summary>
        /// Gets the runtime, MonoGame texture.
        /// </summary>
        /// <value>
        /// The MonoGame texture.
        /// </value>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Reloads the texture from the associated compiled data.  After compilation of the texture
        /// asset, this reloads the MonoGame representation of the texture so that the Texture property
        /// correctly represents a texture based on the compiled data.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if you are attempting to reload a dynamic texture (e.g. a texture constructed
        /// with the constructor that accepts an existing <see cref="Texture2D"/>).
        /// </exception>
        public void ReadyOnGameThread()
        {
            if (this.PlatformData == null)
            {
                return;
            }

            if (this.m_AssetContentManager == null)
            {
                throw new InvalidOperationException("Unable to reload dynamic texture.");
            }

            if (this.m_AssetContentManager != null && this.PlatformData != null)
            {
                using (var stream = new MemoryStream(this.PlatformData.Data))
                {
                    this.m_AssetContentManager.SetStream(this.Name, stream);
                    this.m_AssetContentManager.Purge(this.Name);
                    try
                    {
                        var newTexture = this.m_AssetContentManager.Load<Texture2D>(this.Name);
                        if (newTexture != null)
                        {
                            this.Texture = newTexture;
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

                    this.m_AssetContentManager.UnsetStream(this.Name);
                }
            }
        }

        /// <summary>
        /// Attempt to resolve this asset to the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The target type of the asset.
        /// </typeparam>
        /// <returns>
        /// The current asset as a <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the current asset can not be casted to the designated type.
        /// </exception>
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(TextureAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to TextureAsset.");
        }
    }
}