using Protoinject;

namespace Protogame
{
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