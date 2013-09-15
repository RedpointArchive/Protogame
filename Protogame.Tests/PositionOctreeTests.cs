using System;
using Xunit;

namespace Protogame.Tests
{
    public class PositionOctreeTests
    {
        [Fact]
        public void TestSetAndGet()
        {
            var octree = new PositionOctree<string>();
            octree.Insert("hello", 0, 0, 0);
            Assert.Equal("hello", octree.Find(0, 0, 0));
            octree.Insert("world", -1, -1, -1);
            Assert.Equal("world", octree.Find(-1, -1, -1));
            octree.Insert("blah", -1, 0, -1);
            Assert.Equal("blah", octree.Find(-1, 0, -1));
        }
        
        [Fact]
        public void TestSetAndFastGet()
        {
            var octree = new PositionOctree<string>();
            octree.Insert("hello", 0, 0, 0);
            Assert.Equal("hello", PositionOctreeUtil.GetFast64(octree, 0, 0, 0));
            octree.Insert("world", -1, -1, -1);
            Assert.Equal("world", PositionOctreeUtil.GetFast64(octree, -1, -1, -1));
            octree.Insert("blah", -1, 0, -1);
            Assert.Equal("blah", PositionOctreeUtil.GetFast64(octree, -1, 0, -1));
        }
    }
}

