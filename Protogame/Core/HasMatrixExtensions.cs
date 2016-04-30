using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public static class HasMatrixExtensions
    {
        public static Matrix GetDefaultFinalMatrixImplementation(this IHasMatrix hasMatrix, INode node)
        {
            var parentMatrix = node?.Parent?.UntypedValue as IHasMatrix;
            if (parentMatrix != null)
            {
                return parentMatrix.GetFinalMatrix()*hasMatrix.LocalMatrix;
            }

            return hasMatrix.LocalMatrix;
        }
    }
}