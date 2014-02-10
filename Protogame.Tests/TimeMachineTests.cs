namespace Protogame.Tests
{
    using System.Collections.Generic;
    using Xunit;

    public class TimeMachineTests
    {
        [Fact]
        public void TestPickKeys()
        {
            var machine = new SingleTimeMachine(100);
            var list = new List<int>();
            list.Add(5);
            list.Add(10);
            list.Add(15);
            list.Add(20);
            list.Add(25);
            list.Add(30);
            list.Add(35);
            list.Add(40);
            list.Add(45);
            list.Add(50);

            int previous, next;

            machine.FindSurroundingTickValues(list, 32, out previous, out next);
            Assert.Equal(5, previous);
            Assert.Equal(6, next);

            machine.FindSurroundingTickValues(list, 16, out previous, out next);
            Assert.Equal(2, previous);
            Assert.Equal(3, next);

            machine.FindSurroundingTickValues(list, 2, out previous, out next);
            Assert.Equal(-1, previous);
            Assert.Equal(0, next);

            machine.FindSurroundingTickValues(list, 6, out previous, out next);
            Assert.Equal(0, previous);
            Assert.Equal(1, next);

            machine.FindSurroundingTickValues(list, 11, out previous, out next);
            Assert.Equal(1, previous);
            Assert.Equal(2, next);

            machine.FindSurroundingTickValues(list, 52, out previous, out next);
            Assert.Equal(9, previous);
            Assert.Equal(-1, next);

            machine.FindSurroundingTickValues(list, 0, out previous, out next);
            Assert.Equal(-1, previous);
            Assert.Equal(0, next);

            for (var i = 0; i < list.Count; i++)
            {
                machine.FindSurroundingTickValues(list, (i + 1) * 5, out previous, out next);
                Assert.Equal(i, previous);
                Assert.Equal(i, next);
            }

            for (var i = 0; i < list.Count - 1; i++)
            {
                machine.FindSurroundingTickValues(list, ((i + 1) * 5) + 1, out previous, out next);
                Assert.Equal(i, previous);
                Assert.Equal(i + 1, next);
            }

            for (var i = 1; i < list.Count; i++)
            {
                machine.FindSurroundingTickValues(list, ((i + 1) * 5) - 1, out previous, out next);
                Assert.Equal(i - 1, previous);
                Assert.Equal(i, next);
            }
        }
    }
}