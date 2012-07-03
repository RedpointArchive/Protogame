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
        public const int TILESET_CELL_WIDTH = 16;
        public const int TILESET_CELL_HEIGHT = 16;
        public static readonly int TILESET_WIDTH = (int)Math.Ceiling((float)TILESET_PIXEL_WIDTH / (float)TILESET_CELL_WIDTH);
        public static readonly int TILESET_HEIGHT = (int)Math.Ceiling((float)TILESET_PIXEL_HEIGHT / (float)TILESET_CELL_HEIGHT);
        public const int TILESET_DEPTH = 16;

        private Tile[, ,] m_Tiles;
        private List<Tile> m_LinearList;

        public int[,] DepthAt { get; private set; }

        public Tileset()
        {
            this.m_LinearList = new List<Tile>();
            this.m_Tiles = new Tile[Tileset.TILESET_WIDTH, Tileset.TILESET_HEIGHT, Tileset.TILESET_DEPTH];
            this.DepthAt = new int[Tileset.TILESET_WIDTH, Tileset.TILESET_HEIGHT];
        }

        public Tile this[int x, int y, int z]
        {
            get
            {
                return this.m_Tiles[x, y, z];
            }
            set
            {
                if (value == null)
                    this.m_LinearList.Remove(this.m_Tiles[x, y, z]);
                else
                    this.m_LinearList.Add(value);
                this.m_Tiles[x, y, z] = value;
            }
        }

        public ReadOnlyCollection<Tile> AsLinear()
        {
            return this.m_LinearList.AsReadOnly();
        }
    }
}
