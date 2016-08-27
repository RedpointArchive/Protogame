// ReSharper disable CheckNamespace
#pragma warning disable 1591

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IUpdateContext"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IUpdateContext</interface_ref>
    internal class DefaultUpdateContext : IUpdateContext
    {
        public void Update(IGameContext context)
        {
            // No logic required for our default update context.  Normally
            // you would use this function to initialize properties of
            // the update context based on the state of the game.
        }
    }
}