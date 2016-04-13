namespace Protogame
{
    using System;
    using System.IO;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The font asset.
    /// </summary>
    public class FontAsset : MarshalByRefObject, IAsset
    {
        /// <summary>
        /// The m_ asset content manager.
        /// </summary>
        private readonly IAssetContentManager m_AssetContentManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontAsset"/> class.
        /// </summary>
        /// <param name="assetContentManager">
        /// The asset content manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fontName">
        /// The font name.
        /// </param>
        /// <param name="fontSize">
        /// The font size.
        /// </param>
        /// <param name="useKerning">
        /// The use kerning.
        /// </param>
        /// <param name="spacing">
        /// The spacing.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public FontAsset(
            IAssetContentManager assetContentManager, 
            string name, 
            string fontName, 
            int fontSize, 
            bool useKerning, 
            int spacing, 
            PlatformData data)
        {
            this.Name = name;
            this.FontName = fontName;
            this.FontSize = fontSize;
            this.UseKerning = useKerning;
            this.Spacing = spacing;
            this.PlatformData = data;
            this.m_AssetContentManager = assetContentManager;

            if (this.PlatformData != null)
            {
                try
                {
                    this.ReloadFont();
                }
                catch (NoAssetContentManagerException)
                {
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether compiled only.
        /// </summary>
        /// <value>
        /// The compiled only.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return this.FontName == null;
            }
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public SpriteFont Font { get; private set; }

        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        /// <value>
        /// The font name.
        /// </value>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        /// <value>
        /// The font size.
        /// </value>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the platform data.
        /// </summary>
        /// <value>
        /// The platform data.
        /// </value>
        public PlatformData PlatformData { get; set; }

        /// <summary>
        /// Gets a value indicating whether source only.
        /// </summary>
        /// <value>
        /// The source only.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        /// <summary>
        /// Gets or sets the spacing.
        /// </summary>
        /// <value>
        /// The spacing.
        /// </value>
        public int Spacing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use kerning.
        /// </summary>
        /// <value>
        /// The use kerning.
        /// </value>
        public bool UseKerning { get; set; }

        /// <summary>
        /// The reload font.
        /// </summary>
        public void ReloadFont()
        {
            if (this.m_AssetContentManager != null && this.PlatformData != null)
            {
                using (var stream = new MemoryStream(this.PlatformData.Data))
                {
                    this.m_AssetContentManager.SetStream(this.Name, stream);
                    this.m_AssetContentManager.Purge(this.Name);
                    try
                    {
                        this.Font = this.m_AssetContentManager.Load<SpriteFont>(this.Name);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // This occurs when using assets without a game running, in which case we don't
                        // need the SpriteFont anyway (since we are probably just after the compiled asset).
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
        /// The resolve.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(FontAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to FontAsset.");
        }
    }
}