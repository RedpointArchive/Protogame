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
    
        #region Ground tests
    
        [Fact]
        public void IsNotOnGroundWhenNoEntities()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
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
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            Assert.False(platforming.IsOnGround(
                entity,
                new[] { entity },
                x => true));
        }
    
        [Fact]
        public void IsOnGroundWhenStandingDirectlyOnGround()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var player = this.CreateBoundingBox(200, 200 - 16, 16, 16);
            var ground = this.CreateBoundingBox(0, 200, 400, 16);
            Assert.True(platforming.IsOnGround(
                player,
                new[] { player, ground },
                x => true));
        }
    
        [Fact]
        public void IsNotOnGroundWhenJustAboveTheGround()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var player = this.CreateBoundingBox(200, 200 - 17, 16, 16);
            var ground = this.CreateBoundingBox(0, 200, 400, 16);
            Assert.False(platforming.IsOnGround(
                player,
                new[] { player, ground },
                x => true));
        }
    
        [Fact]
        public void IsOnGroundWhenJustAboveTheGroundAndFalling()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var player = this.CreateBoundingBox(200, 200 - 17, 16, 16, 0, 2);
            var ground = this.CreateBoundingBox(0, 200, 400, 16);
            Assert.True(platforming.IsOnGround(
                player,
                new[] { player, ground },
                x => true));
        }
    
        #endregion Ground tests
    
        #region Speed clamp tests
    
        [Fact]
        public void SpeedIsNotClampedWhenSpeedIsZero()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            platforming.ClampSpeed(boundingBox, 10, 10);
            Assert.Equal(0, boundingBox.XSpeed);
            Assert.Equal(0, boundingBox.YSpeed);
        }
    
        [Fact]
        public void SpeedIsNotClampedWhenSpeedIsUnderLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 5, 5);
            platforming.ClampSpeed(boundingBox, 10, 10);
            Assert.Equal(5, boundingBox.XSpeed);
            Assert.Equal(5, boundingBox.YSpeed);
        }
    
        [Fact]
        public void SpeedIsNotClampedWhenSpeedIsNegativeAndUnderLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, -5, -5);
            platforming.ClampSpeed(boundingBox, 10, 10);
            Assert.Equal(-5, boundingBox.XSpeed);
            Assert.Equal(-5, boundingBox.YSpeed);
        }
    
        [Fact]
        public void SpeedIsNotClampedWhenSpeedIsExactlyAtLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 10, 10);
            platforming.ClampSpeed(boundingBox, 10, 10);
            Assert.Equal(10, boundingBox.XSpeed);
            Assert.Equal(10, boundingBox.YSpeed);
        }
    
        [Fact]
        public void SpeedIsNotClampedWhenSpeedIsNegativeAndExactlyAtLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, -10, -10);
            platforming.ClampSpeed(boundingBox, 10, 10);
            Assert.Equal(-10, boundingBox.XSpeed);
            Assert.Equal(-10, boundingBox.YSpeed);
        }
    
        [Fact]
        public void SpeedIsClampedWhenSpeedIsOverLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 15, 15);
            platforming.ClampSpeed(boundingBox, 10, 10);
            Assert.Equal(10, boundingBox.XSpeed);
            Assert.Equal(10, boundingBox.YSpeed);
        }
    
        [Fact]
        public void SpeedIsClampedWhenSpeedIsNegativeAndOverLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, -15, -15);
            platforming.ClampSpeed(boundingBox, 10, 10);
            Assert.Equal(-10, boundingBox.XSpeed);
            Assert.Equal(-10, boundingBox.YSpeed);
        }
        
        #endregion
    
        #region Gravity tests
    
        [Fact]
        public void GravityIsAppliedWhenGravityIsPositive()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            platforming.ApplyGravity(boundingBox, 0, 10);
            Assert.Equal(10, boundingBox.YSpeed);
            platforming.ApplyGravity(boundingBox, 0, 10);
            Assert.Equal(20, boundingBox.YSpeed);
        }
    
        [Fact]
        public void GravityIsAppliedWhenGravityIsNegative()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            platforming.ApplyGravity(boundingBox, 0, -10);
            Assert.Equal(-10, boundingBox.YSpeed);
            platforming.ApplyGravity(boundingBox, 0, -10);
            Assert.Equal(-20, boundingBox.YSpeed);
        }
        
        #endregion
        
        #region Apply until tests
        
        [Fact]
        public void ApplyUntilRunsUntilLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            var i = 0;
            platforming.ApplyActionUntil(
                boundingBox,
                x => i++,
                x => false,
                10);
            Assert.Equal(10, i);
        }
        
        [Fact]
        public void ApplyUntilRunsUntilCheck()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            var i = 0;
            platforming.ApplyActionUntil(
                boundingBox,
                x => i++,
                x => i >= 10,
                null);
            Assert.Equal(10, i);
        }
        
        #endregion
    }
}

