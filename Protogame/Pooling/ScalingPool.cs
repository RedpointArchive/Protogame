namespace Protogame
{
    using System;
    using System.Collections.Generic;

    public class ScalingPool<T> : IPool<T>
        where T : class
    {
        private readonly Func<T> m_FactoryFunc;

        private readonly int m_Increment;

        private readonly Action<T> m_NewAction;

        private readonly IPoolManager m_PoolManager;

        private readonly List<IPool<T>> m_Pools;

        private readonly Action<T> m_ResetAction;

        public ScalingPool(
            IPoolManager poolManager, 
            string name, 
            int increment, 
            Func<T> factoryFunc, 
            Action<T> resetAction, 
            Action<T> newAction)
        {
            this.Name = name;
            this.m_Pools = new List<IPool<T>>();
            this.m_PoolManager = poolManager;
            this.m_Increment = increment;
            this.m_FactoryFunc = factoryFunc;
            this.m_ResetAction = resetAction;
            this.m_NewAction = newAction;

            // Give us an initial pool.
            this.AllocatePool();
        }

        public int Free { get; private set; }

        public string Name { get; private set; }

        public int NextAvailable { get; private set; }

        public int NextReturn { get; private set; }

        public int Total { get; private set; }

        public T Get()
        {
            if (this.m_Pools[this.m_Pools.Count - 1].Free == 0)
            {
                this.AllocatePool();
            }

            this.Free -= 1;

            return this.m_Pools[this.m_Pools.Count - 1].Get();
        }

        public void Release(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var currentPool = this.m_Pools[this.m_Pools.Count - 1];

            currentPool.Release(instance);

            this.Free += 1;

            if (this.m_Pools.Count > 1 && currentPool.Free == currentPool.Total)
            {
                this.m_Pools.Remove(currentPool);

                this.Free -= this.m_Increment;
                this.Total -= this.m_Increment;
            }
        }

        public void ReleaseAll()
        {
            foreach (var pool in this.m_Pools)
            {
                pool.ReleaseAll();
            }

            this.m_Pools.Clear();

            this.AllocatePool();

            this.Free = this.m_Increment;
            this.Total = this.m_Increment;
        }

        private void AllocatePool()
        {
            this.m_Pools.Add(
                this.m_PoolManager.NewPool(
                    this.Name + this.m_Pools.Count, 
                    this.m_Increment, 
                    this.m_FactoryFunc, 
                    this.m_ResetAction, 
                    this.m_NewAction));

            this.Free += this.m_Increment;
            this.Total += this.m_Increment;
        }
    }
}