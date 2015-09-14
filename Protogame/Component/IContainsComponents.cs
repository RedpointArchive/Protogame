using System.Collections.ObjectModel;

namespace Protogame
{
    public interface IContainsComponents
    {
        ReadOnlyCollection<object> PublicComponents { get; } 
    }
}
