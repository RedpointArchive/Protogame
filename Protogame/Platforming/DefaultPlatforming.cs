using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    class DefaultPlatforming : IPlatforming
    {
        private IBoundingBoxUtilities m_BoundingBoxUtilities;
        
        public DefaultPlatforming(IBoundingBoxUtilities boundingBoxUtilities)
        {
            this.m_BoundingBoxUtilities = boundingBoxUtilities;
        }
    
        public void PerformHorizontalAlignment(IBoundingBox entity, int cellWidth, int cellAlignment, int maxAdjust, Action simulateLeft, Action simulateRight)
        {
            // Perform cell alignment.
            float rem = entity.X % cellWidth;
            if (rem > cellWidth - cellAlignment ||
                rem < cellAlignment)
            {
                float targetX = (float)Math.Round(entity.X / cellWidth) * cellWidth;
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
        
        public bool IsOnGround(IBoundingBox entity, IEnumerable<IBoundingBox> entities, Func<IBoundingBox, bool> ground)
        {
            var collidableEntities = entities.Where(ground).ToArray();
            foreach (var collidableEntity in collidableEntities)
            {
                if (this.m_BoundingBoxUtilities.Overlaps(entity, collidableEntity))
                    return true;
            }
            return false;
        }
        
        public void ApplyGravity(IHasVelocity entity, float xGravity, float yGravity)
        {
            entity.XSpeed += xGravity;
            entity.YSpeed += yGravity;
        }
        
        public void ClampSpeed(IHasVelocity entity, float? maximumXSpeed, float? maximumYSpeed)
        {
            if (maximumXSpeed != null)
            {
                if (entity.XSpeed < -Math.Abs(maximumXSpeed.Value))
                    entity.XSpeed = -Math.Abs(maximumXSpeed.Value);
                if (entity.XSpeed > Math.Abs(maximumXSpeed.Value))
                    entity.XSpeed = Math.Abs(maximumXSpeed.Value);
            }
            if (maximumYSpeed != null)
            {
                if (entity.YSpeed < -Math.Abs(maximumYSpeed.Value))
                    entity.YSpeed = -Math.Abs(maximumYSpeed.Value);
                if (entity.YSpeed > Math.Abs(maximumYSpeed.Value))
                    entity.YSpeed = Math.Abs(maximumYSpeed.Value);
            }
        }
    }
}

