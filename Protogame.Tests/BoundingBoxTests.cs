using Ninject;
using Xunit;

namespace Protogame.Tests
{
    public class BoundingBoxTests
    {
        private IBoundingBox CreateBoundingBox(int x, int y, int width, int height, int xspeed = 0, int yspeed = 0)
        {
            return new BoundingBox
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
                XSpeed = xspeed,
                YSpeed = yspeed
            };
        }
    
        [Fact]
        public void IsNotOverlappingWhenNoBoundingBoxes()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            Assert.False(boundingBoxUtilities.Overlaps());
        }
    
        [Fact]
        public void IsNotOverlappingWhenOneBoundingBox()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            Assert.False(boundingBoxUtilities.Overlaps(this.CreateBoundingBox(200, 200, 16, 16)));
        }
    
        [Fact]
        public void IsOverlappingWhenDifferentBoundingBoxes()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            Assert.True(boundingBoxUtilities.Overlaps(
                this.CreateBoundingBox(200, 200, 16, 16),
                this.CreateBoundingBox(200, 200, 16, 16)));
        }
    
        [Fact]
        public void IsNotOverlappingWhenSameBoundingBoxAndNoOtherBoxes()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            Assert.False(boundingBoxUtilities.Overlaps(entity, entity));
        }
    
        [Fact]
        public void IsNotOverlappingWhenSameBoundingBoxAndOtherBoxesAreNotOverlapping()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(400, 400, 16, 16);
            Assert.False(boundingBoxUtilities.Overlaps(entity, entity, other));
        }
    
        [Fact]
        public void IsOverlappingWhenSameBoundingBoxAndOtherBoxesAreOverlapping()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(208, 208, 16, 16);
            Assert.True(boundingBoxUtilities.Overlaps(entity, entity, other));
        }
    
        [Fact]
        public void IsOverlappingWhenDifferentBoxesOverlap()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(204, 204, 16, 16);
            Assert.True(boundingBoxUtilities.Overlaps(entity, other));
        }
    
        [Fact]
        public void IsNotOverlappingWhenBoxesAreStationary()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(216, 200, 16, 16);
            Assert.False(boundingBoxUtilities.Overlaps(entity, other));
        }
    
        [Fact]
        public void IsOverlappingWhenOneBoxIsMovingTowardTheOther()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            var other = this.CreateBoundingBox(216, 200, 16, 16, -2);
            Assert.True(boundingBoxUtilities.Overlaps(entity, other));
        }
    
        [Fact]
        public void IsOverlappingWhenBothBoxesAreMovingTowardEachOther()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            var boundingBoxUtilities = kernel.Get<IBoundingBoxUtilities>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16, 2);
            var other = this.CreateBoundingBox(216, 200, 16, 16, -2);
            Assert.True(boundingBoxUtilities.Overlaps(entity, other));
        }
    }
}

