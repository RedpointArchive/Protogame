using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Protogame
{
    public class Tileset
    {
        public const int TILESET_PIXEL_WIDTH = 4000;
        public const int TILESET_PIXEL_HEIGHT = 4000;
        public const int TILESET_CELL_WIDTH = 32;
        public const int TILESET_CELL_HEIGHT = 32;
        public static readonly int TILESET_WIDTH = (int)Math.Ceiling((float)TILESET_PIXEL_WIDTH / (float)TILESET_CELL_WIDTH);
        public static readonly int TILESET_HEIGHT = (int)Math.Ceiling((float)TILESET_PIXEL_HEIGHT / (float)TILESET_CELL_HEIGHT);
        private const int TILESET_DEPTH = 16;

        private Tile[, ,] m_Tiles;
        private List<Tile> m_LinearList;

        public int[,] DepthAt { get; private set; }

        public Tileset()
        {
            this.m_LinearList = new List<Tile>();
            this.m_Tiles = new Tile[Tileset.TILESET_WIDTH, Tileset.TILESET_HEIGHT, Tileset.TILESET_DEPTH];
            this.DepthAt = new int[Tileset.TILESET_WIDTH, Tileset.TILESET_HEIGHT];
        }

        public virtual Tile this[int x, int y, int z]
        {
            get
            {
                if (x < 0 || x >= this.m_Tiles.GetLength(0) ||
                    y < 0 || y >= this.m_Tiles.GetLength(1) ||
                    z < 0 || z >= this.m_Tiles.GetLength(2))
                    return null;
                return this.m_Tiles[x, y, z];
            }
            set
            {
                if (x < 0 || x >= this.m_Tiles.GetLength(0) ||
                    y < 0 || y >= this.m_Tiles.GetLength(1) ||
                    z < 0 || z >= this.m_Tiles.GetLength(2))
                    return;
                try
                {
                    if (value == null)
                        this.m_LinearList.Remove(this.m_Tiles[x, y, z]);
                    else
                        this.m_LinearList.Add(value);
                }
                catch (Exception) { }
                this.m_Tiles[x, y, z] = value;
            }
        }

        public ReadOnlyCollection<Tile> AsLinear()
        {
            return this.m_LinearList.AsReadOnly();
        }
    }
}
