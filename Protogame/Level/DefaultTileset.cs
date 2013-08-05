using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultTileset : ITileset
    {
        private IEntity[] m_Entities;
        private int m_CellSizeWidth;
        private int m_CellSizeHeight;
        private int m_TilesetWidth;
        private int m_TilesetHeight;
        
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public IEntity this[int x, int y]
        {
            get
            {
                return this.m_Entities[x + y * this.m_TilesetWidth];
            }
            set
            {
                this.m_Entities[x + y * this.m_TilesetWidth] = value;
            }
        }
    
        public DefaultTileset()
        {
            this.m_Entities = new IEntity[0];
            this.m_CellSizeWidth = 0;
            this.m_CellSizeHeight = 0;
            this.m_TilesetWidth = 0;
            this.m_TilesetHeight = 0;
        }
    
        public void SetSize(Vector2 cellSize, Vector2 tilesetSize)
        {
            this.m_Entities = new IEntity[(int)tilesetSize.X * (int)tilesetSize.Y];
            this.m_CellSizeWidth = (int)cellSize.X;
            this.m_CellSizeHeight = (int)cellSize.Y;
            this.m_TilesetWidth = (int)tilesetSize.X;
            this.m_TilesetHeight = (int)tilesetSize.Y;
        }
        
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            for (var i = 0; i < this.m_Entities.Length; i++)
                this.m_Entities[i].Update(gameContext, updateContext);
        }
        
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            for (var i = 0; i < this.m_Entities.Length; i++)
                this.m_Entities[i].Render(gameContext, renderContext);
        }
    }
}

