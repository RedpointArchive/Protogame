namespace Protogame
{
    public interface IEffectSemantic
    {
        bool ShouldAttachToParameterSet(IEffectParameterSet parameterSet);

        void AttachToParameterSet(IEffectParameterSet parameterSet);

        IEffectSemantic Clone(IEffectParameterSet parameterSet);

        void CacheParameters();

        void OnApply(IRenderContext renderContext);
    }
}