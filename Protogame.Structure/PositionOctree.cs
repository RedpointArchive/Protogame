namespace Protogame.Structure
{
    public class PositionOctree<T> where T : class
    {
        public PositionOctreeNode<T> RootNode = null;

        public PositionOctree()
        {
            this.RootNode = new PositionOctreeNode<T>(0, 64);
        }

        public T Find(long x, long y, long z)
        {
            return this.RootNode.Get(x, y, z);
        }

        public void Insert(T value, long x, long y, long z)
        {
            this.RootNode.Set(value, x, y, z);
        }
    }
}
