using Prototest.Library.Version1;

namespace Protogame.Tests
{
    using System.Collections.Generic;

    public class TimeMachineTests
    {
        private readonly IAssert _assert;

        public TimeMachineTests(IAssert assert)
        {
            _assert = assert;
        }

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
            _assert.Equal(5, previous);
            _assert.Equal(6, next);

            machine.FindSurroundingTickValues(list, 16, out previous, out next);
            _assert.Equal(2, previous);
            _assert.Equal(3, next);

            machine.FindSurroundingTickValues(list, 2, out previous, out next);
            _assert.Equal(-1, previous);
            _assert.Equal(0, next);

            machine.FindSurroundingTickValues(list, 6, out previous, out next);
            _assert.Equal(0, previous);
            _assert.Equal(1, next);

            machine.FindSurroundingTickValues(list, 11, out previous, out next);
            _assert.Equal(1, previous);
            _assert.Equal(2, next);

            machine.FindSurroundingTickValues(list, 52, out previous, out next);
            _assert.Equal(9, previous);
            _assert.Equal(-1, next);

            machine.FindSurroundingTickValues(list, 0, out previous, out next);
            _assert.Equal(-1, previous);
            _assert.Equal(0, next);

            for (var i = 0; i < list.Count; i++)
            {
                machine.FindSurroundingTickValues(list, (i + 1) * 5, out previous, out next);
                _assert.Equal(i, previous);
                _assert.Equal(i, next);
            }

            for (var i = 0; i < list.Count - 1; i++)
            {
                machine.FindSurroundingTickValues(list, ((i + 1) * 5) + 1, out previous, out next);
                _assert.Equal(i, previous);
                _assert.Equal(i + 1, next);
            }

            for (var i = 1; i < list.Count; i++)
            {
                machine.FindSurroundingTickValues(list, ((i + 1) * 5) - 1, out previous, out next);
                _assert.Equal(i - 1, previous);
                _assert.Equal(i, next);
            }
        }
    }
}