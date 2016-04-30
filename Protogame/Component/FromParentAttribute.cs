using Protoinject;

namespace Protogame
{
    /// <summary>
    /// Specifies that the injected argument should be found from the parent's scope.  The
    /// parent of this object is the one injecting it, thus this allows you to search or
    /// depend on other services laterally in the hierarchy.
    /// </summary>
    /// <module>Component</module>
    public class FromParentAttribute : ScopeAttribute
    {
        public override GetScopeFromContext ScopeFromContext
        {
            get { return (current, mapping) => current.Parent ?? current; }
        }

        public override bool UniquePerScope
        {
            get { return true; }
        }
    }
}