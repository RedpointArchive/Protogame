namespace Protogame
{
    /// <summary>
    /// The raw interface for accessing a memory pool.  This 
    /// interface is only used internally.
    /// </summary>
    /// <module>Pooling</module>
    /// <internal>True</internal>
    public interface IRawPool
    {
        string Name { get; }

        int NextAvailable { get; }

        int NextReturn { get; }

        int Free { get; }

        int Total { get; }
    }
}