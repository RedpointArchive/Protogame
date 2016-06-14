using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public static class HasTransformExtensions
    {
        public static IFinalTransform GetAttachedFinalTransformImplementation(this IHasTransform hasTransform, INode node)
        {
            var parentHasTransform = node?.Parent?.UntypedValue as IHasTransform;
            if (parentHasTransform != null)
            {
                return DefaultFinalTransform.Create(parentHasTransform, hasTransform);
            }

            return DefaultFinalTransform.Create(hasTransform);
        }

        public static IFinalTransform GetDetachedFinalTransformImplementation(this IHasTransform hasTransform)
        {
            return DefaultFinalTransform.Create(hasTransform);
        }
    }
}