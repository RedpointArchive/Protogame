namespace Protogame
{
    /// <summary>
    /// An interface which indicates that an object has a transform associated with it.
    /// </summary>
    /// <module>Core API</module>
    public interface IHasTransform
    {
        /// <summary>
        /// The local transform of this object.
        /// </summary>
        ITransform Transform { get; set; }

        /// <summary>
        /// The final transform of this object.
        /// <para>
        /// If you're implementing this property, you probably want to use
        /// <see cref="HasTransformExtensions.GetAttachedFinalTransformImplementation"/> or
        /// <see cref="HasTransformExtensions.GetDetachedFinalTransformImplementation"/>.
        /// </para>
        /// </summary>
        IFinalTransform FinalTransform { get; }
    }
}