namespace Protogame
{
    /// <summary>
    /// The position octree node.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class PositionOctreeNode<T>
        where T : class
    {
        /// <summary>
        /// The m_ nodes.
        /// </summary>
        internal PositionOctreeNode<T>[] m_Nodes;

        /// <summary>
        /// The m_ value.
        /// </summary>
        internal T m_Value;

        /// <summary>
        /// The m_ current depth.
        /// </summary>
        private readonly int m_CurrentDepth;

        /// <summary>
        /// The m_ mask depth.
        /// </summary>
        private readonly long m_MaskDepth;

        /// <summary>
        /// The m_ maximal depth.
        /// </summary>
        private readonly int m_MaximalDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionOctreeNode{T}"/> class.
        /// </summary>
        /// <param name="currentDepth">
        /// The current depth.
        /// </param>
        /// <param name="maximalDepth">
        /// The maximal depth.
        /// </param>
        public PositionOctreeNode(int currentDepth, int maximalDepth)
        {
            this.m_Value = null;
            this.m_CurrentDepth = currentDepth;
            this.m_MaximalDepth = maximalDepth;
            this.m_MaskDepth = 0x1L << (this.m_MaximalDepth - this.m_CurrentDepth - 1);
            this.m_Nodes = new PositionOctreeNode<T>[8] { null, null, null, null, null, null, null, null };
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Get(long x, long y, long z)
        {
            PositionOctreeNode<T> current = this;
            while (current != null && current.m_CurrentDepth != current.m_MaximalDepth)
            {
                if ((x & current.m_MaskDepth) == 0)
                {
                    if ((y & current.m_MaskDepth) == 0)
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            current = current.m_Nodes[0];
                        }
                        else
                        {
                            current = current.m_Nodes[1];
                        }
                    }
                    else
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            current = current.m_Nodes[2];
                        }
                        else
                        {
                            current = current.m_Nodes[3];
                        }
                    }
                }
                else
                {
                    if ((y & current.m_MaskDepth) == 0)
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            current = current.m_Nodes[4];
                        }
                        else
                        {
                            current = current.m_Nodes[5];
                        }
                    }
                    else
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            current = current.m_Nodes[6];
                        }
                        else
                        {
                            current = current.m_Nodes[7];
                        }
                    }
                }
            }

            if (current == null)
            {
                return null;
            }

            return current.m_Value;
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        public void Set(T value, long x, long y, long z)
        {
            PositionOctreeNode<T> current = this;
            while (current != null && current.m_CurrentDepth != current.m_MaximalDepth)
            {
                if ((x & current.m_MaskDepth) == 0)
                {
                    if ((y & current.m_MaskDepth) == 0)
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            if (current.m_Nodes[0] == null)
                            {
                                current.m_Nodes[0] = current.CreateSubnode(-1, -1, -1);
                            }

                            current = current.m_Nodes[0];
                        }
                        else
                        {
                            if (current.m_Nodes[1] == null)
                            {
                                current.m_Nodes[1] = current.CreateSubnode(-1, -1, 1);
                            }

                            current = current.m_Nodes[1];
                        }
                    }
                    else
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            if (current.m_Nodes[2] == null)
                            {
                                current.m_Nodes[2] = current.CreateSubnode(-1, 1, -1);
                            }

                            current = current.m_Nodes[2];
                        }
                        else
                        {
                            if (current.m_Nodes[3] == null)
                            {
                                current.m_Nodes[3] = current.CreateSubnode(-1, 1, 1);
                            }

                            current = current.m_Nodes[3];
                        }
                    }
                }
                else
                {
                    if ((y & current.m_MaskDepth) == 0)
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            if (current.m_Nodes[4] == null)
                            {
                                current.m_Nodes[4] = current.CreateSubnode(1, -1, -1);
                            }

                            current = current.m_Nodes[4];
                        }
                        else
                        {
                            if (current.m_Nodes[5] == null)
                            {
                                current.m_Nodes[5] = current.CreateSubnode(1, -1, 1);
                            }

                            current = current.m_Nodes[5];
                        }
                    }
                    else
                    {
                        if ((z & current.m_MaskDepth) == 0)
                        {
                            if (current.m_Nodes[6] == null)
                            {
                                current.m_Nodes[6] = current.CreateSubnode(1, 1, -1);
                            }

                            current = current.m_Nodes[6];
                        }
                        else
                        {
                            if (current.m_Nodes[7] == null)
                            {
                                current.m_Nodes[7] = current.CreateSubnode(1, 1, 1);
                            }

                            current = current.m_Nodes[7];
                        }
                    }
                }
            }

            current.m_Value = value;
        }

        /// <summary>
        /// The create subnode.
        /// </summary>
        /// <param name="relX">
        /// The rel x.
        /// </param>
        /// <param name="relY">
        /// The rel y.
        /// </param>
        /// <param name="relZ">
        /// The rel z.
        /// </param>
        /// <returns>
        /// The <see cref="PositionOctreeNode"/>.
        /// </returns>
        private PositionOctreeNode<T> CreateSubnode(long relX, long relY, long relZ)
        {
            return new PositionOctreeNode<T>(this.m_CurrentDepth + 1, this.m_MaximalDepth);
        }
    }
}