namespace Protogame
{
    using System.Linq;

    /// <summary>
    /// The focus extensions.
    /// </summary>
    public static class FocusExtensions
    {
        /// <summary>
        /// The blur.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public static void Blur(this IContainer container)
        {
            DefocusGraph(container);
            container.Focused = false;
        }

        /// <summary>
        /// The focus.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public static void Focus(this IContainer container)
        {
            DefocusGraph(container);
            container.Focused = true;
        }

        /// <summary>
        /// The defocus graph.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void DefocusGraph(IContainer container)
        {
            DefocusNode(GetRootContainer(container));
        }

        /// <summary>
        /// The defocus node.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void DefocusNode(IContainer container)
        {
            container.Focused = false;
            foreach (var child in container.Children.Where(x => x != null))
            {
                DefocusNode(child);
            }
        }

        /// <summary>
        /// The get root container.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="IContainer"/>.
        /// </returns>
        private static IContainer GetRootContainer(IContainer container)
        {
            var current = container;
            while (current.Parent != null)
            {
                current = current.Parent;
            }

            return current;
        }
    }
}