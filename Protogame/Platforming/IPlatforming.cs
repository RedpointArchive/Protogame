using System;
using System.Collections.Generic;

namespace Protogame
{
    public interface IPlatforming
    {
        /// <summary>
        /// Aligns the entity horizontally to a grid, invoking the specified actions when the entity needs
        /// to shift left or right to align to the grid.
        /// </summary>
        /// <param name="entity">The entity to align.</param>
        /// <param name="cellWidth">The cell width.</param>
        /// <param name="cellAlignment">The cell alignment.</param>
        /// <param name="maxAdjust">The maximum adjustment permitted.</param>
        /// <param name="simulateLeft">The action which simulates movement to the left.</param>
        /// <param name="simulateRight">The action which simulated movement to the right.</param>
        void PerformHorizontalAlignment(IBoundingBox entity, int cellWidth, int cellAlignment, int maxAdjust, Action simulateLeft, Action simulateRight);
        
        /// <summary>
        /// Determines whether this instance is on the ground by comparing it's bounding box with the
        /// bounding boxes of collidable entities.
        /// </summary>
        /// <returns><c>true</c> if this instance is on the ground.</returns>
        /// <param name="entity">The entity to determine whether it is on the ground.</param>
        /// <param name="entities">All of the entities to consider (usually the World.Entities property).</param>
        /// <param name="collidable">A function invoked on each entity in the enumerable as to whether it should be considered as ground.</param>
        bool IsOnGround(IBoundingBox entity, IEnumerable<IBoundingBox> entities, Func<IBoundingBox, bool> ground);
        
        /// <summary>
        /// Performs action a number of times either until the maximum is reached or the check returns true.
        /// </summary>
        /// <param name="entity">The entity to apply the action to.</param>
        /// <param name="action">The action to apply.</param> 
        /// <param name="check">The check to determine whether the criteria is met.</param>
        /// <param name="maximum">The maximum number of times to perform action, or null to rely only on the check.</param>
        void ApplyActionUntil(IBoundingBox entity, Action<IBoundingBox> action, Func<IBoundingBox, bool> check, int? maximum);
        
        /// <summary>
        /// Applies the specified gravity to the entity.
        /// </summary>
        /// <param name="entity">The entity to apply gravity to.</param>
        /// <param name="xGravity">The X gravity to apply.</param>
        /// <param name="yGravity">The Y gravity to apply.</param>
        void ApplyGravity(IHasVelocity entity, float xGravity, float yGravity);
        
        /// <summary>
        /// Clamps the speed of an entity to the specified values.
        /// </summary>
        /// <param name="entity">The entity whose speed should be clamped.</param>
        /// <param name="maximumXSpeed">The maximum positive or negative X speed, or null to indicate no clamping should be done.</param>
        /// <param name="maximumYSpeed">The maximum positive or negative Y speed, or null to indicate no clamping should be done.</param>
        void ClampSpeed(IHasVelocity entity, float? maximumXSpeed, float? maximumYSpeed);
    }
}

