
namespace Protogame
{
    public class EntityTile : Tile, IEntity
    {
        protected EntityTile()
        {
            this.Width = Tileset.TILESET_CELL_WIDTH;
            this.Height = Tileset.TILESET_CELL_HEIGHT;
            this.TX = -1;
            this.TY = -1;
        }

        public virtual void Update(IUpdateContext context)
        {
        }

        public virtual void Render(IRenderContext context)
        {
        }
    }
}
