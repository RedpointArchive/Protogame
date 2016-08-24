using System;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// Specifies that the injected argument should be found from the game world's scope.  This
    /// searches directly up the tree until a game world is found, or returns null if no game world
    /// was found.
    /// <para>
    /// The context of the dependency injection plan must be an object that is instantiated as a
    /// dependency of the world or created with a factory that is injected into the world or one
    /// of it's dependencies.
    /// </para>
    /// </summary>
    /// <module>Component</module>
    public class FromWorldAttribute : ScopeAttribute
    {
        public override GetScopeFromContext ScopeFromContext
        {
            get
            {
                return (current, mapping) =>
                {
                    var c = current;
                    while (c != null && !typeof (IWorld).IsAssignableFrom(c.Type))
                    {
                        c = c.Parent;
                    }
                    return c;
                };
            }
        }

        public override bool UniquePerScope
        {
            get { return true; }
        }
    }
}