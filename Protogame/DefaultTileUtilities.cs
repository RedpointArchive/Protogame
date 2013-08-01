using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultTileUtilities : ITileUtilities
    {
        private IRenderUtilities m_RenderUtilities;
        private IAssetManager m_AssetManager;
    
        public DefaultTileUtilities(
            IRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
        }
        
        public void RenderTile(ITileEntity entity, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderTexture(
                renderContext,
                new Vector2(entity.X, entity.Y),
                entity.Tileset.Texture,
                new Vector2(entity.Width, entity.Height),
                sourceArea: new Rectangle(
                    entity.TX * entity.Tileset.CellWidth,
                    entity.TY * entity.Tileset.CellHeight,
                    entity.Tileset.CellWidth,
                    entity.Tileset.CellHeight));
        }
        
        public void InitializeTile(ITileEntity entity, string tilesetAssetName)
        {
            entity.Tileset = this.m_AssetManager.Get<TilesetAsset>(tilesetAssetName);
            entity.X *= entity.Tileset.CellWidth;
            entity.Y *= entity.Tileset.CellHeight;
            entity.Width = entity.Tileset.CellWidth;
            entity.Height = entity.Tileset.CellHeight;
        }
    }
}

