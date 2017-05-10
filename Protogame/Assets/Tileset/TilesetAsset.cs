namespace Protogame
{
    public class TilesetAsset : IAsset
    {
        private IAssetReference<TextureAsset> _texture;
        
        private string _textureName;
        
        public TilesetAsset(
            string name,
            string textureName,
            IAssetReference<TextureAsset> texture, 
            int cellWidth, 
            int cellHeight)
        {
            Name = name;
            _textureName = textureName;
            _texture = texture;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }
        
        public int CellHeight { get; }
        
        public int CellWidth { get; }
        
        public string Name { get; }

        public IAssetReference<TextureAsset> Texture => _texture;

        public string TextureName => _textureName;
    }
}