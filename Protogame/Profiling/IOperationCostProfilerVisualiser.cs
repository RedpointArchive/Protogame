namespace Protogame
{
    public interface IOperationCostProfilerVisualiser : IProfilerVisualiser
    {
        int MicrosecondLimit { get; set; }

        int FramesToAnalyse { get; set; }
    }
}