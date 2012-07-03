using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame.RTS;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame.Tiles;
using System.IO;

namespace Protogame.MultiLevel
{
    public abstract class MultiLevelWorld : World
    {
        internal Level[] m_Levels = null;
        internal Tileset[] m_Tilesets = null;
        internal Level m_ActiveLevel = null;

        public MultiLevelWorld()
            : base()
        {
        }

        public Level ActiveLevel
        {
            get
            {
                return this.m_ActiveLevel;
            }
        }

        public override List<IEntity> Entities
        {
            get
            {
                List<IEntity> all = new List<IEntity>();
                foreach (Level l in this.m_Levels)
                    foreach (IEntity e in l.Entities)
                        all.Add(e);
                return all;
            }
        }

        public override Tileset Tileset
        {
            get
            {
                if (this.m_ActiveLevel == null)
                    throw new InvalidOperationException();
                return this.m_Tilesets[Array.IndexOf(this.m_Levels, this.m_ActiveLevel)];
            }
            protected set
            {
                if (value == null)
                    return;
                if (this.m_ActiveLevel == null)
                    throw new InvalidOperationException();
                this.m_Tilesets[Array.IndexOf(this.m_Levels, this.m_ActiveLevel)] = value;
            }
        }

        public void InitMultiLevel(int total)
        {
            this.m_Levels = new Level[total];
            this.m_Tilesets = new Tileset[total];
        }

        protected abstract Dictionary<Level, string> GetLevelToTilesetMapping(string name);

        public void LoadMultiLevel(string name)
        {
            // Get a mapping of level instances to tilesets.
            Dictionary<Level, string> mapping = this.GetLevelToTilesetMapping(name);

            // Perform the actual level switch.
            this.m_Levels = mapping.Keys.ToArray();
            this.m_Tilesets = TilesetXmlLoader.MultiLoad(Path.Combine(World.BaseDirectory, "Resources/" + name + ".oel"), mapping.Values.ToArray());

            for (int i = 0; i < this.m_Levels.Length; i++)
            {
                foreach (Tile t in this.m_Tilesets[i].AsLinear())
                    if (t is MultiLevelEntityTile)
                    {
                        this.m_Levels[i].Entities.Add(t as MultiLevelEntityTile);
                        if (t is IMultiLevelEntity)
                            (t as IMultiLevelEntity).Level = this.m_Levels[i];
                    }
                    else if (t is EntityTile)
                        throw new InvalidOperationException("All entity tiles must derive from MultiLevelEntityTile for MultiLevelWorld.");
                    else if (t is SolidTile && this.m_Levels[i] is PathFindingLevel)
                    {
                        // Mark areas in the level's path finding that are solid.
                        for (int x = (int)(t as SolidTile).X; x < (int)(t as SolidTile).X + (t as SolidTile).Width; x += Tileset.TILESET_CELL_WIDTH)
                            for (int y = (int)(t as SolidTile).Y; y < (int)(t as SolidTile).Y + (t as SolidTile).Height; y += Tileset.TILESET_CELL_HEIGHT)
                                (this.m_Levels[i] as PathFindingLevel).PathMarkTile(x, y, true);
                    }
            }

            // Set initial active level.
            this.m_ActiveLevel = this.m_Levels[0];
        }

        public override void LoadLevel(string name)
        {
            throw new ProtogameException("LoadLevel not supported in multi-level world; use LoadMultiLevel instead.");
        }
    }
}
