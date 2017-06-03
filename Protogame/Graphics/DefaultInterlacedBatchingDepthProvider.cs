#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Protogame
{
    public class DefaultInterlacedBatchingDepthProvider : IInterlacedBatchingDepthProvider
    {
        private float _currentDepth;

        public float GetDepthForCurrentInstance()
        {
            var d = _currentDepth;
            _currentDepth -= 0.00001f;
            if (_currentDepth <= 0)
            {
                _currentDepth = 0f;
            }
            return d;
        }

        public void Reset()
        {
            _currentDepth = 1f;
        }
    }
}
