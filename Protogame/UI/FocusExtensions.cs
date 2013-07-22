using System.Linq;

namespace Protogame
{
    public static class FocusExtensions
    {
        public static void Focus(this IContainer container)
        {
            DefocusGraph(container);
            container.Focused = true;
        }

        public static void Blur(this IContainer container)
        {
            DefocusGraph(container);
            container.Focused = false;
        }

        private static IContainer GetRootContainer(IContainer container)
        {
            var current = container;
            while (current.Parent != null)
                current = current.Parent;
            return current;
        }

        private static void DefocusGraph(IContainer container)
        {
            System.Console.WriteLine("Defocusing graph...");
            DefocusNode(GetRootContainer(container));
        }

        private static void DefocusNode(IContainer container)
        {
            System.Console.WriteLine("Defocusing " + container.GetType().FullName);
            container.Focused = false;
            foreach (var child in container.Children.Where(x => x != null))
                DefocusNode(child);
        }
    }
}

