using Prototest.Library.Version1;

namespace Protogame.Tests
{
    public class PositionOctreeTests
    {
        private readonly IAssert _assert;

        public PositionOctreeTests(IAssert assert)
        {
            _assert = assert;
        }

        public void TestSetAndGet()
        {
            var octree = new PositionOctree<string>();
            octree.Insert("hello", 0, 0, 0);
            _assert.Equal("hello", octree.Find(0, 0, 0));
            octree.Insert("world", -1, -1, -1);
            _assert.Equal("world", octree.Find(-1, -1, -1));
            octree.Insert("blah", -1, 0, -1);
            _assert.Equal("blah", octree.Find(-1, 0, -1));
        }
        
        public void TestSetAndFastGet()
        {
            var octree = new PositionOctree<string>();
            octree.Insert("hello", 0, 0, 0);
            _assert.Equal("hello", PositionOctreeUtil.GetFast64(octree, 0, 0, 0));
            octree.Insert("world", -1, -1, -1);
            _assert.Equal("world", PositionOctreeUtil.GetFast64(octree, -1, -1, -1));
            octree.Insert("blah", -1, 0, -1);
            _assert.Equal("blah", PositionOctreeUtil.GetFast64(octree, -1, 0, -1));
        }
    }
}

