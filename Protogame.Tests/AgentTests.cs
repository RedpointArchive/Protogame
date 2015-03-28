namespace Protogame.Tests
{
    using Microsoft.Xna.Framework;
    using Xunit;

    public class AgentTests
    {
        [Fact]
        public void Unproject()
        {
            var agent = new Agent(new TestEntity(), null, 0);
            agent.Position = new Vector2(100, 100);

            agent.Heading = new Vector2(1, 0);

            Assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(110, 100))));
            Assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(90, 100))));
            Assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(100, 110))));
            Assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(100, 90))));

            agent.Heading = new Vector2(-1, 0);

            Assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(110, 100))));
            Assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(90, 100))));
            Assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(100, 110))));
            Assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(100, 90))));

            agent.Heading = new Vector2(0, 1);

            Assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(110, 100))));
            Assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(90, 100))));
            Assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(100, 110))));
            Assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(100, 90))));

            agent.Heading = new Vector2(0, -1);

            Assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(110, 100))));
            Assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(90, 100))));
            Assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(100, 110))));
            Assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(100, 90))));
        }

        [Fact]
        public void Project()
        {
            var agent = new Agent(new TestEntity(), null, 0);
            agent.Position = new Vector2(100, 100);

            agent.Heading = new Vector2(1, 0);

            Assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(10, 0)));
            Assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(-10, 0)));
            Assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(0, 10)));
            Assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(0, -10)));

            agent.Heading = new Vector2(-1, 0);

            Assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(10, 0)));
            Assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(-10, 0)));
            Assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(0, 10)));
            Assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(0, -10)));

            agent.Heading = new Vector2(0, -1);

            Assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(10, 0)));
            Assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(-10, 0)));
            Assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(0, 10)));
            Assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(0, -10)));

            agent.Heading = new Vector2(0, 1);

            Assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(10, 0)));
            Assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(-10, 0)));
            Assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(0, 10)));
            Assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(0, -10)));
        }

        private Vector2 Rounded(Vector2 src)
        {
            return new Vector2(
                (float)System.Math.Round(src.X),
                (float)System.Math.Round(src.Y));
        }

        private class TestEntity : Entity
        {
        }
    }
}