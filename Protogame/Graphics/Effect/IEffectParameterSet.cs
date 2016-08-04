namespace Protogame
{
    public interface IEffectParameterSet
    {
        IEffectWritableParameter this[int index] { get; }

        IEffectWritableParameter this[string name] { get; }

        bool IsLocked { get; }

        void Lock(IRenderContext renderContext);

        int GetStateHash();

        bool HasSemantic<T>() where T : IEffectSemantic;

        T GetSemantic<T>() where T : IEffectSemantic;
    }
}