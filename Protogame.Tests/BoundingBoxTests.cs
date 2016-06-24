using Microsoft.Xna.Framework;
using Protoinject;
using Prototest.Library.Version1;

namespace Protogame.Tests
{
    public class BoundingBoxTests
    {
        private readonly IAssert _assert;

        public BoundingBoxTests(IAssert assert)
        {
            _assert = assert;
        }

        private IBoundingBox CreateBoundingBox(int x, int y, int width, int height, int xspeed = 0, int yspeed = 0)
        {
            var bb = new BoundingBox
            {
                Width = width,
                Height = height,
                XSpeed = xspeed,
                YSpeed = yspeed
            };
            bb.Transform.Assign(new DefaultTransform { LocalPosition = new Vector3(x, y, 0) });
            return bb;
        }
    
        private IBoundingBox Create3DBoundingBox(int x, int y, int z, int width, int height, int depth, int xspeed = 0, int yspeed = 0, int zspeed = 0)
        {
            var bb = new BoundingBox
            {
                Width = width,
                Height = height,
                Depth = depth,
                XSpeed = xspeed,
                YSpeed = yspeed,
                ZSpeed = zspeed
            };
            bb.Transform.Assign(new DefaultTransform { LocalPosition = new Vector3(x, y, z) });
            return bb;
        }
        
        public void IsNotOverlappingWhenNoBoundingBoxes()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.False(boundingBoxUtilities.Overlaps());
        }
    
        public void IsNotOverlappingWhenOneBoundingBox()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.False(boundingBoxUtilities.Overlaps(this.CreateBoundingBox(200, 200, 16, 16)));
        }
    
        public void IsOverlappingWhenDifferentBoundingBoxes()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.True(boundingBoxUtilities.Overlaps(
                this.CreateBoundingBox(200, 200, 16, 16),
                this.CreateBoundingBox(200, 200, 16, 16)));
        }
    
        public void IsNotOverlappingWhenSameBoundingBoxAndNoOtherBoxes()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            _assert.False(boundingBoxUtilities.Overlaps(entity, entity));
        }
    
        public void IsNotOverlappingWhenSameBoundingBoxAndOtherBoxesAreNotOverlapping()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(400, 400, 16, 16);
            _assert.False(boundingBoxUtilities.Overlaps(entity, entity, other));
        }
    
        public void IsOverlappingWhenSameBoundingBoxAndOtherBoxesAreOverlapping()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(208, 208, 16, 16);
            _assert.True(boundingBoxUtilities.Overlaps(entity, entity, other));
        }
    
        public void IsOverlappingWhenDifferentBoxesOverlap()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(204, 204, 16, 16);
            _assert.True(boundingBoxUtilities.Overlaps(entity, other));
        }
    
        public void IsNotOverlappingWhenBoxesAreStationary()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(216, 200, 16, 16);
            _assert.False(boundingBoxUtilities.Overlaps(entity, other));
        }
    
        public void IsOverlappingWhenOneBoxIsMovingTowardTheOther()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(216, 200, 16, 16, -2);
            _assert.True(boundingBoxUtilities.Overlaps(entity, other));
        }
    
        public void IsOverlappingWhenBothBoxesAreMovingTowardEachOther()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16, 2);
            var other = this.CreateBoundingBox(216, 200, 16, 16, -2);
            _assert.True(boundingBoxUtilities.Overlaps(entity, other));
        }
    
        public void IsOverlappingOnlyWhen3DBoxesIntersectOuterBefore()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.False(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 100),
                this.Create3DBoundingBox(0, 0, 0, 200, 200, 0)));
        }
    
        public void IsOverlappingOnlyWhen3DBoxesIntersectOuter()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.True(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 100),
                this.Create3DBoundingBox(0, 0, 50, 200, 200, 0)));
        }
        
        public void IsOverlappingOnlyWhen3DBoxesIntersectMiddle()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.True(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 100),
                this.Create3DBoundingBox(0, 0, 100, 200, 200, 0)));
        }
        
        public void IsOverlappingOnlyWhen3DBoxesIntersectInner()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.False(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 100),
                this.Create3DBoundingBox(0, 0, 150, 200, 200, 0)));
        }
        
        public void IsOverlappingOnlyWhen3DBoxesIntersectInnerAfter()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.False(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 100),
                this.Create3DBoundingBox(0, 0, 200, 200, 200, 0)));
        }
        
        public void IsNotOverlappingWhenBoxesAreNextToEachOther()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.False(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 50),
                this.Create3DBoundingBox(100, 50, 50, 50, 50, 50)));
            _assert.False(boundingBoxUtilities.Overlaps(
                this.CreateBoundingBox(50, 50, 50, 50),
                this.CreateBoundingBox(100, 50, 50, 50)));
        }
    
        public void IsOverlappingWhenBoxWithZeroDimensionIsNextToAnother()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            _assert.False(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 50),
                this.Create3DBoundingBox(100, 50, 50, 0, 50, 50)));
            _assert.True(boundingBoxUtilities.Overlaps(
                this.Create3DBoundingBox(50, 50, 50, 50, 50, 50),
                this.Create3DBoundingBox(50, 50, 50, 0, 50, 50)));
            _assert.False(boundingBoxUtilities.Overlaps(
                this.CreateBoundingBox(50, 50, 50, 50),
                this.CreateBoundingBox(100, 50, 0, 50)));
            _assert.True(boundingBoxUtilities.Overlaps(
                this.CreateBoundingBox(50, 50, 50, 50),
                this.CreateBoundingBox(50, 50, 0, 50)));
        }
    }
}

