using System;

namespace Protogame
{
    public class NeedsComponentInHierarchyToContinueException : Exception
    {
        public Type ComponentType { get; set; }

        public NeedsComponentInHierarchyToContinueException(Type componentType)
        {
            ComponentType = componentType;
        }
    }
}
