using Microsoft.Xna.Framework;
using Protoinject;
using Prototest.Library.Version1;

namespace Protogame.Tests
{
    public class PlatformingModuleTests
    {
        private readonly IAssert _assert;

        private IBoundingBox CreateBoundingBox(int x, int y, int width, int height, int xspeed = 0, int yspeed = 0)
        {
            return new BoundingBox
            {
                LocalMatrix = Matrix.CreateTranslation(x, y, 0),
                Width = width,
                Height = height,
                XSpeed = xspeed,
                YSpeed = yspeed
            };
        }

        public PlatformingModuleTests(IAssert assert)
        {
            _assert = assert;
        }

        #region Ground tests
    
        public void IsNotOnGroundWhenNoEntities()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            _assert.False(platforming.IsOnGround(
                this.CreateBoundingBox(200, 200, 16, 16),
                new IBoundingBox[0],
                x => true));
        }
        
        public void IsNotOnGroundWhenOnlyEntity()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var entity = this.CreateBoundingBox(200, 200, 16, 16);
            _assert.False(platforming.IsOnGround(
                entity,
                new[] { entity },
                x => true));
        }
    
        public void IsOnGroundWhenStandingDirectlyOnGround()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var player = this.CreateBoundingBox(200, 200 - 16, 16, 16);
            var ground = this.CreateBoundingBox(0, 200, 400, 16);
            _assert.True(platforming.IsOnGround(
                player,
                new[] { player, ground },
                x => true));
        }
    
        public void IsNotOnGroundWhenJustAboveTheGround()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var player = this.CreateBoundingBox(200, 200 - 17, 16, 16);
            var ground = this.CreateBoundingBox(0, 200, 400, 16);
            _assert.False(platforming.IsOnGround(
                player,
                new[] { player, ground },
                x => true));
        }
    
        public void IsOnGroundWhenJustAboveTheGroundAndFalling()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            var player = this.CreateBoundingBox(200, 200 - 17, 16, 16, 0, 2);
            var ground = this.CreateBoundingBox(0, 200, 400, 16);
            _assert.True(platforming.IsOnGround(
                player,
                new[] { player, ground },
                x => true));
        }
    
        #endregion Ground tests
    
        #region Speed clamp tests
    
        public void SpeedIsNotClampedWhenSpeedIsZero()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            platforming.ClampSpeed(boundingBox, 10, 10);
            _assert.Equal(0, boundingBox.XSpeed);
            _assert.Equal(0, boundingBox.YSpeed);
        }
    
        public void SpeedIsNotClampedWhenSpeedIsUnderLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 5, 5);
            platforming.ClampSpeed(boundingBox, 10, 10);
            _assert.Equal(5, boundingBox.XSpeed);
            _assert.Equal(5, boundingBox.YSpeed);
        }
    
        public void SpeedIsNotClampedWhenSpeedIsNegativeAndUnderLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, -5, -5);
            platforming.ClampSpeed(boundingBox, 10, 10);
            _assert.Equal(-5, boundingBox.XSpeed);
            _assert.Equal(-5, boundingBox.YSpeed);
        }
    
        public void SpeedIsNotClampedWhenSpeedIsExactlyAtLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 10, 10);
            platforming.ClampSpeed(boundingBox, 10, 10);
            _assert.Equal(10, boundingBox.XSpeed);
            _assert.Equal(10, boundingBox.YSpeed);
        }
    
        public void SpeedIsNotClampedWhenSpeedIsNegativeAndExactlyAtLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, -10, -10);
            platforming.ClampSpeed(boundingBox, 10, 10);
            _assert.Equal(-10, boundingBox.XSpeed);
            _assert.Equal(-10, boundingBox.YSpeed);
        }
    
        public void SpeedIsClampedWhenSpeedIsOverLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 15, 15);
            platforming.ClampSpeed(boundingBox, 10, 10);
            _assert.Equal(10, boundingBox.XSpeed);
            _assert.Equal(10, boundingBox.YSpeed);
        }
    
        public void SpeedIsClampedWhenSpeedIsNegativeAndOverLimit()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, -15, -15);
            platforming.ClampSpeed(boundingBox, 10, 10);
            _assert.Equal(-10, boundingBox.XSpeed);
            _assert.Equal(-10, boundingBox.YSpeed);
        }
        
        #endregion
    
        #region Gravity tests
    
        public void GravityIsAppliedWhenGravityIsPositive()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            platforming.ApplyGravity(boundingBox, 0, 10);
            _assert.Equal(10, boundingBox.YSpeed);
            platforming.ApplyGravity(boundingBox, 0, 10);
            _assert.Equal(20, boundingBox.YSpeed);
        }
    
        public void GravityIsAppliedWhenGravityIsNegative()
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogamePlatformingIoCModule>();
            var platforming = kernel.Get<IPlatforming>();
            
            var boundingBox = this.CreateBoundingBox(0, 0, 0, 0, 0, 0);
            platforming.ApplyGravity(boundingBox, 0, -10);
            _assert.Equal(-10, boundingBox.YSpeed);
            platforming.ApplyGravity(boundingBox, 0, -10);
            _assert.Equal(-20, boundingBox.YSpeed);
        }
        
        #endregion
        
        #region Apply until tests
        
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
            _assert.Equal(10, i);
        }
        
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
            _assert.Equal(10, i);
        }
        
        #endregion
    }
}

