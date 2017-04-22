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
        
        public DefaultTileUtilities(I2DRenderUtilities renderUtilities, IAssetManager assetManager)
        {
            _renderUtilities = renderUtilities;
            _assetManager = assetManager;
        }
        
        public void InitializeTile(ITileEntity entity, string tilesetAssetName)
        {
            entity.Tileset = _assetManager.Get<TilesetAsset>(tilesetAssetName);
        }
        
        public void RenderTile(ITileEntity entity, IRenderContext renderContext)
        {
            if (!entity.Tileset.IsReady)
            {
                return;
            }

            var tileset = entity.Tileset.Asset;

            if (!entity.AppliedTilesetSettings)
            {
                entity.Transform.LocalScale *= new Vector3(tileset.CellWidth, tileset.CellHeight, 1);
                entity.Width = tileset.CellWidth;
                entity.Height = tileset.CellHeight;
                entity.AppliedTilesetSettings = true;
            }

            _renderUtilities.RenderTexture(
                renderContext, 
                new Vector2(entity.Transform.LocalPosition.X * tileset.CellWidth, entity.Transform.LocalPosition.Y * tileset.CellHeight),
                tileset.Texture, 
                new Vector2(entity.Width, entity.Height), 
                sourceArea:
                    new Rectangle(
                        entity.TX * tileset.CellWidth, 
                        entity.TY * tileset.CellHeight,
                        tileset.CellWidth,
                        tileset.CellHeight));
        }
    }
}