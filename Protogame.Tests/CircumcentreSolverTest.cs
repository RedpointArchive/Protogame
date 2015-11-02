using Prototest.Library.Version1;

namespace Protogame.Math.Tests
{
    public class CircumcentreSolverTest
    {
        private readonly IAssert _assert;

        public CircumcentreSolverTest(IAssert assert)
        {
            _assert = assert;
        }

        private void AssertSphere(double[] a, double[] b, double[] c, double[] d, double[] centre, double radius)
        {
            CircumcentreSolver solver = new CircumcentreSolver(a, b, c, d);
            _assert.Equal(centre[0], solver.Centre[0]);
            _assert.Equal(centre[1], solver.Centre[1]);
            _assert.Equal(centre[2], solver.Centre[2]);
            _assert.Equal(radius, solver.Radius);
        }
        
        public void TestSolver()
        {
            this.AssertSphere(
                new double[] { 0, 1, 0 },
                new double[] { 0, -1, 0 },
                new double[] { 4, 0, 0 },
                new double[] { 0, 0, 1 },
                new double[] { 1.875, 0, 0 },
                2.125);
            this.AssertSphere(
                new double[] { 0, 1, 0 },
                new double[] { 0, -1, 0 },
                new double[] { 1, 0, 0 },
                new double[] { 0, 0, 1 },
                new double[] { 0, 0, 0 },
                1);
        }
    }
}
