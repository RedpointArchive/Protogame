using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IPerPixelCollidableComponent
    {
        /// <summary>
        /// This method is called by the per-pixel collision system when a collision involving one of this component's
        /// parents and another object with a per-pixel collision component starts occurring.
        /// </summary>
        /// <param name="gameContext">The current game context, or null if running on a server.</param>
        /// <param name="serverContext">The current server context, or null if running on a client.</param>
        /// <param name="updateContext">The current update context.</param>
        /// <param name="obj1">The owner of the first per-pixel collision component.  This is NOT necessarily one of the component's parents.</param>
        /// <param name="obj2">The owner of the second per-pixel collision component.  This is NOT necessarily one of the component's parents.</param>
        void PerPixelCollision(IGameContext gameContext, IServerContext serverContext, IUpdateContext updateContext, object obj1, object obj2);

    }
}
