namespace Protogame
{
    /// <summary>
    /// An interface which indicates this implementation contains
    /// entities within it.  Since the entities are contained in the
    /// dependency injection hierarchy, there aren't actually any 
    /// properties on this interface.  Instead you'll generally inject
    /// the <see cref="IEntityManagement"/> to manage entities within
    /// the hierarchy.
    /// </summary>
    public interface IContainsEntities
    {
    }
}
