namespace Protogame
{
    using System;

    /// <summary>
    /// An implementation of an <see cref="IPool{T}"/> which manages an array
    /// of objects.  This implementation does not allocate the objects for
    /// you; you need to initialize an array of objects which is then used
    /// in this implementation.
    /// <para>
    /// It's recommended that you use <see cref="ScalingPool{T}"/> instead,
    /// creating it via <see cref="IPoolManager"/>.
    /// </para>
    /// </summary>
    /// <module>Pooling</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IPool</interface_ref>
    public class Pool<T> : IPool<T>
        where T : class
    {
        private readonly string m_Name;

        private readonly T[] m_PooledInstances;

        private readonly Action<T> m_ResetAction;

        private readonly Action<T> m_NewAction;

        private readonly object m_PoolLock;

        private int m_NextAvailable;

        private int m_NextReturn;

        public Pool(string name, T[] pooledInstances, Action<T> resetAction, Action<T> newAction = null)
        {
            if (pooledInstances.Length == 0)
            {
                throw new InvalidOperationException("Pooled instance array must be at least 1 in size");
            }

            this.m_Name = name;
            this.m_PoolLock = new object();
            this.m_PooledInstances = pooledInstances;
            this.m_ResetAction = resetAction;
            this.m_NewAction = newAction;
            this.m_NextAvailable = 0;
            this.m_NextReturn = -1;
            this.Free = this.m_PooledInstances.Length;
            this.Total = this.m_PooledInstances.Length;
        }

        public int Free { get; private set; }

        public int Total { get; private set; }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        public int NextAvailable
        {
            get
            {
                return this.m_NextAvailable;
            }
        }

        public int NextReturn
        {
            get
            {
                return this.m_NextReturn;
            }
        }

        public T Get()
        {
            lock (this.m_PoolLock)
            {
                if (this.m_NextAvailable == -1)
                {
                    throw new OutOfMemoryException(
                        "Pool '" + this.m_Name
                        + "' has exceeded number of available instances.");
                }

                this.Free--;

                // Get the instance and remove it from the pool.
                var value = this.m_PooledInstances[this.m_NextAvailable];

                // If we don't yet have a position to release values to, tell it the next
                // release position is where we just allocated from.
                if (this.m_NextReturn == -1)
                {
                    this.m_NextReturn = this.m_NextAvailable;
                }

                // Increment the available counter.
                this.m_NextAvailable++;
                if (this.m_NextAvailable == this.m_NextReturn)
                {
                    // No more available until next release.
                    this.m_NextAvailable = -1;
                }
                else if (this.m_NextAvailable >= this.m_PooledInstances.Length)
                {
                    if (this.m_NextReturn > 0)
                    {
                        // The return location is greater than the 0th position,
                        // so we've previously returned instances to 0.
                        this.m_NextAvailable = 0;
                    }
                    else
                    {
                        // No more available until next release.
                        this.m_NextAvailable = -1;
                    }
                }

                // Fire the new action if it is set.
                if (this.m_NewAction != null)
                {
                    this.m_NewAction(value);
                }

                return value;
            }
        }

        public void Release(T instance)
        {
            lock (this.m_PoolLock)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                if (this.m_NextReturn == -1)
                {
                    throw new OutOfMemoryException("Pool '" + this.m_Name + "' can not release any more objects");
                }

                this.Free++;

                // Reset instance state.
                this.m_ResetAction(instance);

                // Return the value to the return location.
                this.m_PooledInstances[this.m_NextReturn] = instance;

                // If we can't allocate out, then we can now.
                if (this.m_NextAvailable == -1)
                {
                    this.m_NextAvailable = this.m_NextReturn;
                }

                // Increment the return counter.
                this.m_NextReturn++;
                if (this.m_NextReturn >= this.m_PooledInstances.Length)
                {
                    this.m_NextReturn = 0;
                }

                if (this.m_NextReturn == this.m_NextAvailable)
                {
                    // We've filled up the last available spot, there's no more return
                    // locations until we allocate more out.
                    this.m_NextReturn = -1;
                }
            }
        }

        public void ReleaseAll()
        {
            lock (this.m_PoolLock)
            {
                this.m_NextAvailable = 0;
                this.m_NextReturn = -1;
                this.Free = this.m_PooledInstances.Length;

                foreach (var x in this.m_PooledInstances)
                {
                    this.m_ResetAction(x);
                }
            }
        }
    }
}