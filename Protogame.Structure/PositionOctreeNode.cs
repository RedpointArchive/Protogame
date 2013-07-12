using Tychaia.Globals;

namespace Protogame.Structure
{
    public class PositionOctreeNode<T> where T : class
    {
        private T m_Value;
        private PositionOctreeNode<T>[] m_Nodes;
        private int m_CurrentDepth;
        private int m_MaximalDepth;

        // 0 = -1, -1, -1
        // 1 = -1, -1,  1
        // 2 = -1,  1, -1
        // 3 = -1,  1,  1
        // 4 =  1, -1, -1
        // 5 =  1, -1,  1
        // 6 =  1,  1, -1
        // 7 =  1,  1,  1

        public PositionOctreeNode(int currentDepth, int maximalDepth)
        {
            this.m_Value = null;
            this.m_CurrentDepth = currentDepth;
            this.m_MaximalDepth = maximalDepth;
            this.m_Nodes = new PositionOctreeNode<T>[8]
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            };
        }

        public T Get(long x, long y, long z)
        {
            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "GET " + x + ", " + y + ", " + z + "");
            PositionOctreeNode<T> current = this;
            while (current != null && current.m_CurrentDepth != current.m_MaximalDepth)
            {
                if ((x & current.GetMaskAtDepth()) == 0)
                {
                    if ((y & current.GetMaskAtDepth()) == 0)
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            current = current.m_Nodes[0] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "0 - ");
                        }
                        else
                        {
                            current = current.m_Nodes[1] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "1 - ");
                        }
                    }
                    else
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            current = current.m_Nodes[2] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "2 - ");
                        }
                        else
                        {
                            current = current.m_Nodes[3] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "3 - ");
                        }
                    }
                }
                else
                {
                    if ((y & current.GetMaskAtDepth()) == 0)
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            current = current.m_Nodes[4] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "4 - ");
                        }
                        else
                        {
                            current = current.m_Nodes[5] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "5 - ");
                        }
                    }
                    else
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            current = current.m_Nodes[6] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "6 - ");
                        }
                        else
                        {
                            current = current.m_Nodes[7] ?? null;
                            FilteredConsole.Write(FilterCategory.OctreeGetTracing, "7 - ");
                        }
                    }
                }

                if (current != null && current.m_CurrentDepth % 8 == 0)
                    FilteredConsole.WriteLine(FilterCategory.OctreeGetTracing, "");
            }
            FilteredConsole.WriteLine(FilterCategory.OctreeGetTracing, "");
            if (current == null)
                return null;
            return current.m_Value;
        }

        private long GetMaskAtDepth()
        {
            return 0x1L << (m_MaximalDepth - m_CurrentDepth - 1);
        }

        private PositionOctreeNode<T> CreateSubnode(long relX, long relY, long relZ)
        {
            return new PositionOctreeNode<T>(this.m_CurrentDepth + 1, this.m_MaximalDepth);
        }

        public void Set(T value, long x, long y, long z)
        {
            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "SET " + x + ", " + y + ", " + z + "");
            PositionOctreeNode<T> current = this;
            while (current != null && current.m_CurrentDepth != current.m_MaximalDepth)
            {
                if ((x & current.GetMaskAtDepth()) == 0)
                {
                    if ((y & current.GetMaskAtDepth()) == 0)
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            if (current.m_Nodes[0] == null)
                                current.m_Nodes[0] = current.CreateSubnode(-1, -1, -1);
                            current = current.m_Nodes[0];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "0 - ");
                        }
                        else
                        {
                            if (current.m_Nodes[1] == null)
                                current.m_Nodes[1] = current.CreateSubnode(-1, -1, 1);
                            current = current.m_Nodes[1];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "1 - ");
                        }
                    }
                    else
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            if (current.m_Nodes[2] == null)
                                current.m_Nodes[2] = current.CreateSubnode(-1, 1, -1);
                            current = current.m_Nodes[2];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "2 - ");
                        }
                        else
                        {
                            if (current.m_Nodes[3] == null)
                                current.m_Nodes[3] = current.CreateSubnode(-1, 1, 1);
                            current = current.m_Nodes[3];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "3 - ");
                        }
                    }
                }
                else
                {
                    if ((y & current.GetMaskAtDepth()) == 0)
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            if (current.m_Nodes[4] == null)
                                current.m_Nodes[4] = current.CreateSubnode(1, -1, -1);
                            current = current.m_Nodes[4];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "4 - ");
                        }
                        else
                        {
                            if (current.m_Nodes[5] == null)
                                current.m_Nodes[5] = current.CreateSubnode(1, -1, 1);
                            current = current.m_Nodes[5];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "5 - ");
                        }
                    }
                    else
                    {
                        if ((z & current.GetMaskAtDepth()) == 0)
                        {
                            if (current.m_Nodes[6] == null)
                                current.m_Nodes[6] = current.CreateSubnode(1, 1, -1);
                            current = current.m_Nodes[6];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "6 - ");
                        }
                        else
                        {
                            if (current.m_Nodes[7] == null)
                                current.m_Nodes[7] = current.CreateSubnode(1, 1, 1);
                            current = current.m_Nodes[7];
                            FilteredConsole.Write(FilterCategory.OctreeSetTracing, "7 - ");
                        }
                    }
                }
            }

            FilteredConsole.WriteLine(FilterCategory.OctreeSetTracing, "");
            current.m_Value = value;
        }
    }
}
