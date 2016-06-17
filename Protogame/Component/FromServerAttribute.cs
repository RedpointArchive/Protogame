using Protoinject;

namespace Protogame
{
    /// <summary>
    /// Specifies that the injected argument should be found from the server's scope.  This
    /// searches directly up the tree until a server is found, or returns null if no server
    /// was found.
    /// <para>
    /// The context of the dependency injection plan must be an object that is instantiated as a
    /// dependency of the server or created with a factory that is injected into the server or one
    /// of it's dependencies.
    /// </para>
    /// </summary>
    /// <module>Component</module>
    public class FromServerAttribute : ScopeAttribute
    {
        public override GetScopeFromContext ScopeFromContext
        {
            get
            {
                return (current, mapping) =>
                {
                    var c = current;
                    while (c != null && typeof (ICoreServer).IsAssignableFrom(c.Type))
                    {
                        c = current.Parent;
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