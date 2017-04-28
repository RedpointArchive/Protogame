namespace Protogame
{
    using System;

    /// <summary>
    /// The tileset asset.
    /// </summary>
    public class TilesetAsset : IAsset
    {
        /// <summary>
        /// The m_ asset manager.
        /// </summary>
        private readonly IAssetManager m_AssetManager;

        /// <summary>
        /// The m_ texture.
        /// </summary>
        private IAssetReference<TextureAsset> m_Texture;

        /// <summary>
        /// The m_ texture name.
        /// </summary>
        private string m_TextureName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TilesetAsset"/> class.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="textureName">
        /// The texture name.
        /// </param>
        /// <param name="cellWidth">
        /// The cell width.
        /// </param>
        /// <param name="cellHeight">
        /// The cell height.
        /// </param>
        public TilesetAsset(IAssetManager assetManager, string name, string textureName, int cellWidth, int cellHeight)
        {
            this.Name = name;
            this.m_AssetManager = assetManager;
            this.m_TextureName = textureName;
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
        }

        /// <summary>
        /// Gets or sets the cell height.
        /// </summary>
        /// <value>
        /// The cell height.
        /// </value>
        public int CellHeight { get; set; }

        /// <summary>
        /// Gets or sets the cell width.
        /// </summary>
        /// <value>
        /// The cell width.
        /// </value>
        public int CellWidth { get; set; }

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
                return false;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

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
                return false;
            }
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public IAssetReference<TextureAsset> Texture
        {
            get
            {
                if (this.m_Texture == null)
                {
                    this.m_Texture = this.m_AssetManager.Get<TextureAsset>(this.m_TextureName);
                }

                return this.m_Texture;
            }
        }

        /// <summary>
        /// Gets or sets the texture name.
        /// </summary>
        /// <value>
        /// The texture name.
        /// </value>
        public string TextureName
        {
            get
            {
                return this.m_TextureName;
            }

            set
            {
                this.m_Texture = null;
                this.m_TextureName = value;
            }
        }

    }
}