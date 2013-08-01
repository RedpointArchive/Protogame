using System;

namespace Protogame
{
    public class TilesetAsset : MarshalByRefObject, IAsset
    {
        private IAssetManager m_AssetManager;
        private string m_TextureName;
        private TextureAsset m_Texture;
    
        public string Name { get; private set; }
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }
        
        public string TextureName
        {
            get { return this.m_TextureName; }
            set
            {
                this.m_Texture = null;
                this.m_TextureName = value;
            }
        }
        
        public TextureAsset Texture
        {
            get
            {
                if (this.m_Texture == null)
                    this.m_Texture = this.m_AssetManager.TryGet<TextureAsset>(this.m_TextureName);
                return this.m_Texture;
            }
        }

        public TilesetAsset(
            IAssetManager assetManager,
            string name,
            string textureName,
            int cellWidth,
            int cellHeight)
        {
            this.Name = name;
            this.m_AssetManager = assetManager;
            this.m_TextureName = textureName;
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(TilesetAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to TilesetAsset.");
        }
    }
}

