namespace Protogame
{
    public interface IEffectWithSemantics
    {
        bool HasSemantic<T>() where T : IEffectSemantic;
        T GetSemantic<T>() where T : IEffectSemantic;
    }
}