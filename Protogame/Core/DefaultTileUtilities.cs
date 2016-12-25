// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="ITileUtilities"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ITileUtilities</interface_ref>
    public class DefaultTileUtilities : ITileUtilities
    {
        private readonly IAssetManager _assetManager;
        
        private readonly I2DRenderUtilities _renderUtilities;
        
        public DefaultTileUtilities(I2DRenderUtilities renderUtilities, IAssetManagerProvider assetManagerProvider)
        {
            _renderUtilities = renderUtilities;
            _assetManager = assetManagerProvider.GetAssetManager();
        }
        
        public void InitializeTile(ITileEntity entity, string tilesetAssetName)
        {
            entity.Tileset = _assetManager.Get<TilesetAsset>(tilesetAssetName);
            entity.Transform.LocalScale *= new Vector3(entity.Tileset.CellWidth, entity.Tileset.CellHeight, 1);
            entity.Width = entity.Tileset.CellWidth;
            entity.Height = entity.Tileset.CellHeight;
        }
        
        public void RenderTile(ITileEntity entity, IRenderContext renderContext)
        {
            _renderUtilities.RenderTexture(
                renderContext, 
                new Vector2(entity.Transform.LocalPosition.X * entity.Tileset.CellWidth, entity.Transform.LocalPosition.Y * entity.Tileset.CellHeight), 
                entity.Tileset.Texture, 
                new Vector2(entity.Width, entity.Height), 
                sourceArea:
                    new Rectangle(
                        entity.TX * entity.Tileset.CellWidth, 
                        entity.TY * entity.Tileset.CellHeight, 
                        entity.Tileset.CellWidth, 
                        entity.Tileset.CellHeight));
        }
    }
}