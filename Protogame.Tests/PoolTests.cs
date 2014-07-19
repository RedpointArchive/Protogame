namespace Protogame.Tests
{
    using Protogame;
    using Xunit;

    public class PoolTests
    {
        [Fact]
        public void TestIntArray()
        {
            var poolManager = new DefaultPoolManager();

            var pool = poolManager.NewArrayPool<int>("testintarray", 5, 10, x => { });

            Assert.Equal(5, pool.Free);
            Assert.Equal(0, pool.NextAvailable);
            Assert.Equal(-1, pool.NextReturn);

            var a1 = pool.Get();

            Assert.Equal(4, pool.Free);
            Assert.Equal(1, pool.NextAvailable);
            Assert.Equal(0, pool.NextReturn);

            var a2 = pool.Get();

            Assert.Equal(3, pool.Free);
            Assert.Equal(2, pool.NextAvailable);
            Assert.Equal(0, pool.NextReturn);

            var a3 = pool.Get();

            Assert.Equal(2, pool.Free);
            Assert.Equal(3, pool.NextAvailable);
            Assert.Equal(0, pool.NextReturn);

            var a4 = pool.Get();

            Assert.Equal(1, pool.Free);
            Assert.Equal(4, pool.NextAvailable);
            Assert.Equal(0, pool.NextReturn);

            var a5 = pool.Get();

            Assert.Equal(0, pool.Free);
            Assert.Equal(-1, pool.NextAvailable);
            Assert.Equal(0, pool.NextReturn);

            // Ensure instances are unique.
            Assert.NotSame(a1, a2);
            Assert.NotSame(a1, a3);
            Assert.NotSame(a1, a4);
            Assert.NotSame(a1, a5);
            Assert.NotSame(a2, a3);
            Assert.NotSame(a2, a4);
            Assert.NotSame(a2, a5);
            Assert.NotSame(a3, a4);
            Assert.NotSame(a3, a5);
            Assert.NotSame(a4, a5);

            // Test release
            pool.Release(a3);

            Assert.Equal(1, pool.Free);
            Assert.Equal(0, pool.NextAvailable);
            Assert.Equal(1, pool.NextReturn);

            pool.Release(a4);

            Assert.Equal(2, pool.Free);
            Assert.Equal(0, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            var b3 = pool.Get();

            Assert.Equal(1, pool.Free);
            Assert.Equal(1, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            var b4 = pool.Get();

            Assert.Equal(0, pool.Free);
            Assert.Equal(-1, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            // Check same instances
            Assert.Same(a3, b3);
            Assert.Same(a4, b4);

            // Check offset release
            pool.Release(a1);

            Assert.Equal(1, pool.Free);
            Assert.Equal(2, pool.NextAvailable);
            Assert.Equal(3, pool.NextReturn);

            pool.Release(a2);

            Assert.Equal(2, pool.Free);
            Assert.Equal(2, pool.NextAvailable);
            Assert.Equal(4, pool.NextReturn);

            pool.Release(b3);

            Assert.Equal(3, pool.Free);
            Assert.Equal(2, pool.NextAvailable);
            Assert.Equal(0, pool.NextReturn);

            pool.Release(b4);

            Assert.Equal(4, pool.Free);
            Assert.Equal(2, pool.NextAvailable);
            Assert.Equal(1, pool.NextReturn);

            pool.Release(a5);

            Assert.Equal(5, pool.Free);
            Assert.Equal(2, pool.NextAvailable);
            Assert.Equal(-1, pool.NextReturn);

            // Check offset get
            var c1 = pool.Get();

            Assert.Equal(4, pool.Free);
            Assert.Equal(3, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            var c2 = pool.Get();

            Assert.Equal(3, pool.Free);
            Assert.Equal(4, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            var c3 = pool.Get();

            Assert.Equal(2, pool.Free);
            Assert.Equal(0, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            var c4 = pool.Get();

            Assert.Equal(1, pool.Free);
            Assert.Equal(1, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            var c5 = pool.Get();

            Assert.Equal(0, pool.Free);
            Assert.Equal(-1, pool.NextAvailable);
            Assert.Equal(2, pool.NextReturn);

            // Make sure reallocation got the right instances
            Assert.Same(a1, c1);
            Assert.Same(a2, c2);
            Assert.Same(a3, c3);
            Assert.Same(a4, c4);
            Assert.Same(a5, c5);
        }

        private class IntContainer
        {
            public int Value { get; set; }
        }

        [Fact]
        public void TestScalingPool()
        {
            var poolManager = new DefaultPoolManager();

            var scalingPool = poolManager.NewScalingPool<IntContainer>("scaling", 5, x => x.Value = 0);

            Assert.Equal(5, scalingPool.Free);
            Assert.Equal(5, scalingPool.Total);

            var a1 = scalingPool.Get();
            var a2 = scalingPool.Get();
            var a3 = scalingPool.Get();
            var a4 = scalingPool.Get();
            var a5 = scalingPool.Get();

            Assert.Equal(0, scalingPool.Free);
            Assert.Equal(5, scalingPool.Total);

            var b1 = scalingPool.Get();

            Assert.Equal(4, scalingPool.Free);
            Assert.Equal(10, scalingPool.Total);

            scalingPool.Release(b1);

            Assert.Equal(0, scalingPool.Free);
            Assert.Equal(5, scalingPool.Total);

            scalingPool.Release(a5);
            scalingPool.Release(a4);
            scalingPool.Release(a3);
            scalingPool.Release(a2);
            scalingPool.Release(a1);

            Assert.Equal(5, scalingPool.Free);
            Assert.Equal(5, scalingPool.Total);
        }
    }
}
