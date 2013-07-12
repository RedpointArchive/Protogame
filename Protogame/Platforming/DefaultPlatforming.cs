using System;

namespace Protogame.Platforming
{
    class DefaultPlatforming : IPlatforming
    {
        public void PerformHorizontalAlignment(IEntity entity, int cellAlignment, int maxAdjust, Action simulateLeft, Action simulateRight)
        {
            // Perform cell alignment.
            float rem = entity.X % Tileset.TILESET_CELL_WIDTH;
            if (rem > Tileset.TILESET_CELL_WIDTH - cellAlignment ||
                rem < cellAlignment)
            {
                float targetX = (float)Math.Round(entity.X / Tileset.TILESET_CELL_WIDTH) * Tileset.TILESET_CELL_WIDTH;
                if (Math.Abs(targetX - entity.X) > maxAdjust)
                {
                    if (targetX > entity.X)
                        simulateRight();
                    else if (targetX < entity.X)
                        simulateLeft();
                }
                else
                    entity.X = targetX;
            }
        }
    }
}

