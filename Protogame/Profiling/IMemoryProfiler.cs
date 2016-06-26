using System.Collections.Generic;

namespace Protogame
{
    public interface IMemoryProfiler : IProfiler
    {
        Dictionary<string, double> GetMeasuredCosts();

        void ResetMeasuredCosts();
    }
}