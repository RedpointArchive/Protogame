using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultTileUtilities : ITileUtilities
    {
        private IRenderUtilities m_RenderUtilities;
    
        public DefaultTileUtilities(IRenderUtilities renderUtilities)
        {
            this.m_RenderUtilities = renderUtilities;
        }
        
        public void RenderTile(ITileEntity entity, IRenderContext renderContext)
        {
            this.m_RenderUtilities.RenderRectangle(
                renderContext,
                new Rectangle((int)entity.X, (int)entity.Y, (int)entity.Width, (int)entity.Height),
                Color.Brown,
                filled: true);
        }
        
        public void AdjustTileBounds(ITileEntity entity, int tilesetCellWidth, int tilesetCellHeight)
        {
            entity.X *= tilesetCellWidth;
            entity.Y *= tilesetCellHeight;
            entity.Width = tilesetCellWidth;
            entity.Height = tilesetCellHeight;
        }
    }
}

