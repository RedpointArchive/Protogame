using Microsoft.Xna.Framework;

namespace Protogame
{
    public class AbsoluteMatrixComponent : IHasMatrix
    {
        public Matrix LocalMatrix { get; set; }

        public Matrix GetFinalMatrix()
        {
            return LocalMatrix;
        }
    }
}