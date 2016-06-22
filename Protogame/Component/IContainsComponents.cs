using System.Collections.ObjectModel;

namespace Protogame
{
    /// <summary>
    /// An interface that indicates that this object or component contains
    /// sub-components.  All hierarchial components support sub-components.
    /// </summary>
    /// <module>Component</module>
    public interface IContainsComponents
    {
        /// <summary>
        /// A list of components that are attached to this object.
        /// </summary>
        ReadOnlyCollection<object> Components { get; } 
    }
}
