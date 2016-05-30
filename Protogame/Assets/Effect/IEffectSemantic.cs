namespace Protogame
{
    public interface IEffectSemantic
    {
        bool ShouldAttachToEffect(EffectWithSemantics effectWithSemantics);

        void AttachToEffect(EffectWithSemantics effectWithSemantics);

        IEffectSemantic Clone(EffectWithSemantics effectWithSemantics);
        
        void CacheEffectParameters();

        void OnApply();
    }
}