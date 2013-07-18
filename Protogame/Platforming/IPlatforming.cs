using System;

namespace Protogame
{
    public interface IPlatforming
    {
        void PerformHorizontalAlignment(IEntity entity, int cellAlignment, int maxAdjust, Action simulateLeft, Action simulateRight);
    }
}

