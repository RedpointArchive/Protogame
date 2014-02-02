namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default platforming.
    /// </summary>
    internal class DefaultPlatforming : IPlatforming
    {
        /// <summary>
        /// The m_ bounding box utilities.
        /// </summary>
        private readonly IBoundingBoxUtilities m_BoundingBoxUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPlatforming"/> class.
        /// </summary>
        /// <param name="boundingBoxUtilities">
        /// The bounding box utilities.
        /// </param>
        public DefaultPlatforming(IBoundingBoxUtilities boundingBoxUtilities)
        {
            this.m_BoundingBoxUtilities = boundingBoxUtilities;
        }

        /// <summary>
        /// The apply action until.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="check">
        /// The check.
        /// </param>
        /// <param name="maximum">
        /// The maximum.
        /// </param>
        public void ApplyActionUntil(
            IBoundingBox entity, 
            Action<IBoundingBox> action, 
            Func<IBoundingBox, bool> check, 
            int? maximum)
        {
            if (maximum == null)
            {
                while (!check(entity))
                {
                    action(entity);
                }
            }
            else
            {
                for (var i = 0; i < maximum.Value; i++)
                {
                    if (!check(entity))
                    {
                        action(entity);
                    }
                }
            }
        }

        /// <summary>
        /// The apply gravity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="xGravity">
        /// The x gravity.
        /// </param>
        /// <param name="yGravity">
        /// The y gravity.
        /// </param>
        public void ApplyGravity(IHasVelocity entity, float xGravity, float yGravity)
        {
            entity.XSpeed += xGravity;
            entity.YSpeed += yGravity;
        }

        /// <summary>
        /// The apply movement.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="xAmount">
        /// The x amount.
        /// </param>
        /// <param name="yAmount">
        /// The y amount.
        /// </param>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <param name="ground">
        /// The ground.
        /// </param>
        public void ApplyMovement(
            IBoundingBox entity, 
            int xAmount, 
            int yAmount, 
            IEnumerable<IBoundingBox> entities, 
            Func<IBoundingBox, bool> ground)
        {
            var collidableEntities = entities.Where(ground).ToArray();
            for (var x = 0; x < Math.Abs(xAmount); x++)
            {
                entity.X += Math.Sign(xAmount);
                foreach (var other in collidableEntities)
                {
                    if (this.m_BoundingBoxUtilities.Overlaps(entity, other))
                    {
                        entity.X -= Math.Sign(xAmount);
                        goto endX;
                    }
                }
            }

            endX:
            for (var y = 0; y < Math.Abs(yAmount); y++)
            {
                entity.Y += Math.Sign(yAmount);
                foreach (var other in collidableEntities)
                {
                    if (this.m_BoundingBoxUtilities.Overlaps(entity, other))
                    {
                        entity.Y -= Math.Sign(yAmount);
                        goto endY;
                    }
                }
            }

            endY:
            ;
        }

        /// <summary>
        /// The clamp speed.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="maximumXSpeed">
        /// The maximum x speed.
        /// </param>
        /// <param name="maximumYSpeed">
        /// The maximum y speed.
        /// </param>
        public void ClampSpeed(IHasVelocity entity, float? maximumXSpeed, float? maximumYSpeed)
        {
            if (maximumXSpeed != null)
            {
                if (entity.XSpeed < -Math.Abs(maximumXSpeed.Value))
                {
                    entity.XSpeed = -Math.Abs(maximumXSpeed.Value);
                }

                if (entity.XSpeed > Math.Abs(maximumXSpeed.Value))
                {
                    entity.XSpeed = Math.Abs(maximumXSpeed.Value);
                }
            }

            if (maximumYSpeed != null)
            {
                if (entity.YSpeed < -Math.Abs(maximumYSpeed.Value))
                {
                    entity.YSpeed = -Math.Abs(maximumYSpeed.Value);
                }

                if (entity.YSpeed > Math.Abs(maximumYSpeed.Value))
                {
                    entity.YSpeed = Math.Abs(maximumYSpeed.Value);
                }
            }
        }

        /// <summary>
        /// The is on ground.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <param name="ground">
        /// The ground.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsOnGround(IBoundingBox entity, IEnumerable<IBoundingBox> entities, Func<IBoundingBox, bool> ground)
        {
            var entityExtended = new BoundingBox
            {
                X = entity.X, 
                Y = entity.Y + 1, 
                Width = entity.Width, 
                Height = entity.Height, 
                XSpeed = entity.XSpeed, 
                YSpeed = entity.YSpeed
            };
            var collidableEntities = entities.Where(ground).Where(x => x != entity).ToArray();
            foreach (var collidableEntity in collidableEntities)
            {
                if (this.m_BoundingBoxUtilities.Overlaps(entityExtended, collidableEntity))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The perform horizontal alignment.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="cellWidth">
        /// The cell width.
        /// </param>
        /// <param name="cellAlignment">
        /// The cell alignment.
        /// </param>
        /// <param name="maxAdjust">
        /// The max adjust.
        /// </param>
        /// <param name="simulateLeft">
        /// The simulate left.
        /// </param>
        /// <param name="simulateRight">
        /// The simulate right.
        /// </param>
        public void PerformHorizontalAlignment(
            IBoundingBox entity, 
            int cellWidth, 
            int cellAlignment, 
            int maxAdjust, 
            Action simulateLeft, 
            Action simulateRight)
        {
            // Perform cell alignment.
            float rem = entity.X % cellWidth;
            if (rem > cellWidth - cellAlignment || rem < cellAlignment)
            {
                float targetX = (float)Math.Round(entity.X / cellWidth) * cellWidth;
                if (Math.Abs(targetX - entity.X) > maxAdjust)
                {
                    if (targetX > entity.X)
                    {
                        simulateRight();
                    }
                    else if (targetX < entity.X)
                    {
                        simulateLeft();
                    }
                }
                else
                {
                    entity.X = targetX;
                }
            }
        }
    }
}