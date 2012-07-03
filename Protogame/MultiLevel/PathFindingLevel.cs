using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using DeenGames.Utils.AStarPathFinder;
using Microsoft.Xna.Framework;

namespace Protogame.MultiLevel
{
    public abstract class PathFindingLevel : Level
    {
        byte[,] m_PathFindingGrid = null;

        public PathFindingLevel(MultiLevelWorld world)
            : base(world)
        {
            int width = PathFinderHelper.RoundToNearestPowerOfTwo(Tileset.TILESET_WIDTH);
            int height = PathFinderHelper.RoundToNearestPowerOfTwo(Tileset.TILESET_HEIGHT);
            this.m_PathFindingGrid = new byte[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    this.m_PathFindingGrid[x, y] = (byte)PathFinderHelper.EMPTY_TILE;
        }

        public void PathMarkTile(int x, int y, bool full)
        {
            this.m_PathFindingGrid[x / Tileset.TILESET_CELL_WIDTH, y / Tileset.TILESET_CELL_WIDTH] =
                full ? (byte)PathFinderHelper.BLOCKED_TILE : (byte)PathFinderHelper.EMPTY_TILE;
        }

        public List<PathFinderNode> PathFindFast(Vector2 current, Vector2 target)
        {
            return new PathFinderFast(this.m_PathFindingGrid) { Formula = HeuristicFormula.Manhattan }.FindPath(
                new DeenGames.Utils.Point((int)current.X / Tileset.TILESET_CELL_WIDTH, (int)current.Y / Tileset.TILESET_CELL_HEIGHT),
                new DeenGames.Utils.Point((int)target.X / Tileset.TILESET_CELL_WIDTH, (int)target.Y / Tileset.TILESET_CELL_HEIGHT));
        }

        protected void DrawDebugPathFindingGrid(GameContext context, XnaGraphics graphics)
        {
            for (int x = 0; x < this.m_PathFindingGrid.GetLength(0); x++)
                for (int y = 0; y < this.m_PathFindingGrid.GetLength(1); y++)
                    if (this.m_PathFindingGrid[x, y] == (byte)PathFinderHelper.BLOCKED_TILE)
                        graphics.DrawRectangle(new Rectangle(x * 16, y * 16, 16, 16), new Color(255, 0, 0, 128));
                    else if (this.m_PathFindingGrid[x, y] == (byte)PathFinderHelper.EMPTY_TILE)
                        graphics.DrawRectangle(new Rectangle(x * 16, y * 16, 16, 16), new Color(0, 255, 0, 128));
                    else
                        graphics.DrawRectangle(new Rectangle(x * 16, y * 16, 16, 16), new Color(0, 0, 255, 128));
        }
    }
}
