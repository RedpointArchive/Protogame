using System;

namespace Protogame
{
    class DefaultUpdateContext : IUpdateContext
    {
        public void Update(IGameContext context)
        {
            // No logic required for our default update context.  Normally
            // you would use this function to initialize properties of
            // the update context based on the state of the game.
        }
    }
}

