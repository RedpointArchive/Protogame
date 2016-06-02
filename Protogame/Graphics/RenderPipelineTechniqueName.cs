namespace Protogame
{
    /// <summary>
    /// A well-known list of technique names used in the render pipeline.  This allows
    /// effects to standardize on the names of the techniques they use, and have them
    /// work inside predefined render pipeline passes.
    /// </summary>
    public static class RenderPipelineTechniqueName
    {
        /// <summary>
        /// The name of techniques intended for use in forward rendering.
        /// </summary>
        public const string Forward = "Forward";

        /// <summary>
        /// The name of techniques intended for use in deferred rendering.
        /// </summary>
        public const string Deferred = "Deferred";

        /// <summary>
        /// The name of techniques intended for use in batched 2D rendering.
        /// </summary>
        public const string Batched2D = "Batched2D";

        /// <summary>
        /// The name of techniques intended for use in direct 2D rendering.
        /// </summary>
        public const string Direct2D = "Direct2D";

        /// <summary>
        /// The name of techniques intended for use in canvas rendering.
        /// </summary>
        public const string Canvas = "Canvas";

        /// <summary>
        /// The name of techniques intended for use in post processing.
        /// </summary>
        public const string PostProcess = "PostProcess";
    }
}
