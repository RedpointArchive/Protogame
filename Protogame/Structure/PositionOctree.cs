namespace Protogame
{
    /// <summary>
    /// An octree used for storing objects at fixed positions.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object being stored.
    /// </typeparam>
    public class PositionOctree<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionOctree{T}"/> class.
        /// </summary>
        public PositionOctree()
        {
            this.RootNode = new PositionOctreeNode<T>(0, 64);
        }

        /// <summary>
        /// Gets the root node in the octree.
        /// </summary>
        /// <value>
        /// The root node in the octree.
        /// </value>
        public PositionOctreeNode<T> RootNode { get; private set; }

        /// <summary>
        /// Find an object at the specified X, Y and Z position.
        /// </summary>
        /// <param name="x">
        /// The X position.
        /// </param>
        /// <param name="y">
        /// The Y position.
        /// </param>
        /// <param name="z">
        /// The Z position.
        /// </param>
        /// <returns>
        /// The <see cref="T"/> at that position, or null if it doesn't exist.
        /// </returns>
        public T Find(long x, long y, long z)
        {
            return this.RootNode.Get(x, y, z);
        }

        /// <summary>
        /// Insert an object at the specified X, Y and Z position.
        /// </summary>
        /// <param name="value">
        /// The value to insert.
        /// </param>
        /// <param name="x">
        /// The X position.
        /// </param>
        /// <param name="y">
        /// The Y position.
        /// </param>
        /// <param name="z">
        /// The Z position.
        /// </param>
        public void Insert(T value, long x, long y, long z)
        {
            this.RootNode.Set(value, x, y, z);
        }
    }
}