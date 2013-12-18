namespace Protogame
{
    public interface IAsset
    {
        string Name { get; }

        bool SourceOnly { get; }
        bool CompiledOnly { get;  }

        T Resolve<T>() where T : class, IAsset;
    }
}
