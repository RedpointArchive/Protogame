using Protoinject;

namespace Protogame
{
    /// <summary>
    /// Extension methods which assist developers in writing classes that
    /// implement <see cref="IHasTransform"/>.
    /// </summary>
    /// <module>Core API</module>
    public static class HasTransformExtensions
    {
        /// <summary>
        /// Returns a final transform by combining the final transform of the parent in the hierarchy (if the parent
        /// node has a transform), and the local transform of this object.  You should use this method to implement
        /// <see cref="IHasTransform.FinalTransform"/> if your object resides in the hierarchy.
        /// </summary>
        /// <param name="hasTransform">The current object.</param>
        /// <param name="node">
        /// The node in the dependency injection hierarchy that points to this object.  This value
        /// can be obtained by injecting <see cref="INode"/> into the constructor of your object.
        /// </param>
        /// <returns>A final computed transform.</returns>
        public static IFinalTransform GetAttachedFinalTransformImplementation(this IHasTransform hasTransform, INode node)
        {
            var parentHasTransform = node?.Parent?.UntypedValue as IHasTransform;
            if (parentHasTransform != null)
            {
                return DefaultFinalTransform.Create(parentHasTransform, hasTransform);
            }

            return DefaultFinalTransform.Create(hasTransform);
        }

        /// <summary>
        /// Gets a final transform which is just representative of the local transform.  This method should be used
        /// sparingly, but is intended when either you know the parent of this object will have no transform (i.e.
        /// you are implementing an entity which resides directly in the world), or when there's no way for the caller
        /// to know it's position in the hierarchy.
        /// </summary>
        /// <param name="hasTransform">The current object.</param>
        /// <returns>A final computed transform.</returns>
        public static IFinalTransform GetDetachedFinalTransformImplementation(this IHasTransform hasTransform)
        {
            return DefaultFinalTransform.Create(hasTransform);
        }
    }
}