namespace Protogame
{
    public interface IEffectTechniqueCollection
    {
        IEffectTechnique this[int index] { get; }

        IEffectTechnique this[string name] { get; }
    }
}