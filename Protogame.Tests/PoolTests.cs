using Prototest.Library.Version1;

namespace Protogame.Tests
{
    using Protogame;

    public class PoolTests
    {
        private readonly IAssert _assert;

        public PoolTests(IAssert assert)
        {
            _assert = assert;
        }

        public void TestIntArray()
        {
            var poolManager = new DefaultPoolManager();

            var pool = poolManager.NewArrayPool<int>("testintarray", 5, 10, x => { });

            _assert.Equal(5, pool.Free);
            _assert.Equal(0, pool.NextAvailable);
            _assert.Equal(-1, pool.NextReturn);

            var a1 = pool.Get();

            _assert.Equal(4, pool.Free);
            _assert.Equal(1, pool.NextAvailable);
            _assert.Equal(0, pool.NextReturn);

            var a2 = pool.Get();

            _assert.Equal(3, pool.Free);
            _assert.Equal(2, pool.NextAvailable);
            _assert.Equal(0, pool.NextReturn);

            var a3 = pool.Get();

            _assert.Equal(2, pool.Free);
            _assert.Equal(3, pool.NextAvailable);
            _assert.Equal(0, pool.NextReturn);

            var a4 = pool.Get();

            _assert.Equal(1, pool.Free);
            _assert.Equal(4, pool.NextAvailable);
            _assert.Equal(0, pool.NextReturn);

            var a5 = pool.Get();

            _assert.Equal(0, pool.Free);
            _assert.Equal(-1, pool.NextAvailable);
            _assert.Equal(0, pool.NextReturn);

            // Ensure instances are unique.
            _assert.NotSame(a1, a2);
            _assert.NotSame(a1, a3);
            _assert.NotSame(a1, a4);
            _assert.NotSame(a1, a5);
            _assert.NotSame(a2, a3);
            _assert.NotSame(a2, a4);
            _assert.NotSame(a2, a5);
            _assert.NotSame(a3, a4);
            _assert.NotSame(a3, a5);
            _assert.NotSame(a4, a5);

            // Test release
            pool.Release(a3);

            _assert.Equal(1, pool.Free);
            _assert.Equal(0, pool.NextAvailable);
            _assert.Equal(1, pool.NextReturn);

            pool.Release(a4);

            _assert.Equal(2, pool.Free);
            _assert.Equal(0, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            var b3 = pool.Get();

            _assert.Equal(1, pool.Free);
            _assert.Equal(1, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            var b4 = pool.Get();

            _assert.Equal(0, pool.Free);
            _assert.Equal(-1, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            // Check same instances
            _assert.Same(a3, b3);
            _assert.Same(a4, b4);

            // Check offset release
            pool.Release(a1);

            _assert.Equal(1, pool.Free);
            _assert.Equal(2, pool.NextAvailable);
            _assert.Equal(3, pool.NextReturn);

            pool.Release(a2);

            _assert.Equal(2, pool.Free);
            _assert.Equal(2, pool.NextAvailable);
            _assert.Equal(4, pool.NextReturn);

            pool.Release(b3);

            _assert.Equal(3, pool.Free);
            _assert.Equal(2, pool.NextAvailable);
            _assert.Equal(0, pool.NextReturn);

            pool.Release(b4);

            _assert.Equal(4, pool.Free);
            _assert.Equal(2, pool.NextAvailable);
            _assert.Equal(1, pool.NextReturn);

            pool.Release(a5);

            _assert.Equal(5, pool.Free);
            _assert.Equal(2, pool.NextAvailable);
            _assert.Equal(-1, pool.NextReturn);

            // Check offset get
            var c1 = pool.Get();

            _assert.Equal(4, pool.Free);
            _assert.Equal(3, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            var c2 = pool.Get();

            _assert.Equal(3, pool.Free);
            _assert.Equal(4, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            var c3 = pool.Get();

            _assert.Equal(2, pool.Free);
            _assert.Equal(0, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            var c4 = pool.Get();

            _assert.Equal(1, pool.Free);
            _assert.Equal(1, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            var c5 = pool.Get();

            _assert.Equal(0, pool.Free);
            _assert.Equal(-1, pool.NextAvailable);
            _assert.Equal(2, pool.NextReturn);

            // Make sure reallocation got the right instances
            _assert.Same(a1, c1);
            _assert.Same(a2, c2);
            _assert.Same(a3, c3);
            _assert.Same(a4, c4);
            _assert.Same(a5, c5);
        }

        private class IntContainer
        {
            public int Value { get; set; }
        }
        
        public void TestScalingPool()
        {
            var poolManager = new DefaultPoolManager();

            var scalingPool = poolManager.NewScalingPool<IntContainer>("scaling", 5, x => x.Value = 0);

            _assert.Equal(5, scalingPool.Free);
            _assert.Equal(5, scalingPool.Total);

            var a1 = scalingPool.Get();
            var a2 = scalingPool.Get();
            var a3 = scalingPool.Get();
            var a4 = scalingPool.Get();
            var a5 = scalingPool.Get();

            _assert.Equal(0, scalingPool.Free);
            _assert.Equal(5, scalingPool.Total);

            var b1 = scalingPool.Get();

            _assert.Equal(4, scalingPool.Free);
            _assert.Equal(10, scalingPool.Total);

            scalingPool.Release(b1);

            _assert.Equal(0, scalingPool.Free);
            _assert.Equal(5, scalingPool.Total);

            scalingPool.Release(a5);
            scalingPool.Release(a4);
            scalingPool.Release(a3);
            scalingPool.Release(a2);
            scalingPool.Release(a1);

            _assert.Equal(5, scalingPool.Free);
            _assert.Equal(5, scalingPool.Total);
        }
    }
}
