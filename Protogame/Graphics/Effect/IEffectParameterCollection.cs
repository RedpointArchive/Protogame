namespace Protogame
{
    public interface IEffectParameterCollection
    {
        IEffectParameter this[int index] { get; }

        IEffectParameter this[string name] { get; }
    }
}