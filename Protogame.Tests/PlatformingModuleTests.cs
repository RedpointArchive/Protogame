using Ninject;
using Xunit;

namespace Protogame.Tests
{
    public class PlatformingModuleTests
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
        public void IsNotOnGroundWhenNoEntities()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            Assert.False(platforming.IsOnGround(
                this.CreateBoundingBox(200, 200, 16, 16),
                new IBoundingBox[0],
                x => true));
        }
        
        [Fact]
        public void IsNotOnGroundWhenOnlyEntity()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            Assert.False(platforming.IsOnGround(
                entity,
                new[] { entity },
                x => true));
        }
    
        [Fact]
        public void IsOnGroundWhenCollidingWithGround()
        {
        }
    }
}

