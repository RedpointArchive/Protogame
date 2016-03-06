using Prototest.Library.Version1;

namespace Protogame.Tests
{
    using Microsoft.Xna.Framework;

    public class AgentTests
    {
        private readonly IAssert _assert;

        public AgentTests(IAssert assert)
        {
            _assert = assert;
        }

        public void Unproject()
        {
            var agent = new Agent(null, new TestEntity(), null, 0);
            agent.Position = new Vector2(100, 100);

            agent.Heading = new Vector2(1, 0);

            _assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(110, 100))));
            _assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(90, 100))));
            _assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(100, 110))));
            _assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(100, 90))));

            agent.Heading = new Vector2(-1, 0);

            _assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(110, 100))));
            _assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(90, 100))));
            _assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(100, 110))));
            _assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(100, 90))));

            agent.Heading = new Vector2(0, 1);

            _assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(110, 100))));
            _assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(90, 100))));
            _assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(100, 110))));
            _assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(100, 90))));

            agent.Heading = new Vector2(0, -1);

            _assert.Equal(new Vector2(0, 10), Rounded(agent.Unproject(new Vector2(110, 100))));
            _assert.Equal(new Vector2(0, -10), Rounded(agent.Unproject(new Vector2(90, 100))));
            _assert.Equal(new Vector2(-10, 0), Rounded(agent.Unproject(new Vector2(100, 110))));
            _assert.Equal(new Vector2(10, 0), Rounded(agent.Unproject(new Vector2(100, 90))));
        }
        
        public void Project()
        {
            var agent = new Agent(null, new TestEntity(), null, 0);
            agent.Position = new Vector2(100, 100);

            agent.Heading = new Vector2(1, 0);

            _assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(10, 0)));
            _assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(-10, 0)));
            _assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(0, 10)));
            _assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(0, -10)));

            agent.Heading = new Vector2(-1, 0);

            _assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(10, 0)));
            _assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(-10, 0)));
            _assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(0, 10)));
            _assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(0, -10)));

            agent.Heading = new Vector2(0, -1);

            _assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(10, 0)));
            _assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(-10, 0)));
            _assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(0, 10)));
            _assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(0, -10)));

            agent.Heading = new Vector2(0, 1);

            _assert.Equal(new Vector2(100, 110), agent.Project(new Vector2(10, 0)));
            _assert.Equal(new Vector2(100, 90), agent.Project(new Vector2(-10, 0)));
            _assert.Equal(new Vector2(90, 100), agent.Project(new Vector2(0, 10)));
            _assert.Equal(new Vector2(110, 100), agent.Project(new Vector2(0, -10)));
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