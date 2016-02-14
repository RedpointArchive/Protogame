namespace Protogame
{
    using System;
    using System.Linq;

    /// <summary>
    /// The default implementation of an <see cref="IPoolManager"/>.
    /// </summary>
    /// <module>Pooling</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IPoolManager</interface_ref>
    public class DefaultPoolManager : IPoolManager
    {
        public IPool<T> NewPool<T>(string name, int size, Action<T> resetAction) where T : class, new()
        {
            return this.NewPool(name, size, () => new T(), resetAction, null);
        }

        public IPool<T> NewPool<T>(string name, int size, Func<T> factoryFunc, Action<T> resetAction) where T : class
        {
            return this.NewPool(name, size, factoryFunc, resetAction, null);
        }

        public IPool<T> NewPool<T>(string name, int size, Func<T> factoryFunc, Action<T> resetAction, Action<T> newAction) where T : class
        {
            return new Pool<T>(name, Enumerable.Range(0, size).Select(x => factoryFunc()).ToArray(), resetAction, newAction);
        }

        public IPool<T> NewScalingPool<T>(string name, int increment, Action<T> resetAction) where T : class, new()
        {
            return this.NewScalingPool(name, increment, () => new T(), resetAction, null);
        }

        public IPool<T> NewScalingPool<T>(string name, int increment, Func<T> factoryFunc, Action<T> resetAction) where T : class
        {
            return this.NewScalingPool(name, increment, factoryFunc, resetAction, null);
        }

        public IPool<T> NewScalingPool<T>(string name, int increment, Func<T> factoryFunc, Action<T> resetAction, Action<T> newAction) where T : class
        {
            return new ScalingPool<T>(this, name, increment, factoryFunc, resetAction, newAction);
        }

        public IPool<T[]> NewArrayPool<T>(string name, int size, int arraySize, Action<T[]> resetAction)
        {
            return new Pool<T[]>(name, Enumerable.Range(0, size).Select(x => new T[arraySize]).ToArray(), resetAction);
        }

        public IPool<T[]> NewScalingArrayPool<T>(string name, int increment, int arraySize, Action<T[]> resetAction)
        {
            return new ScalingPool<T[]>(this, name, increment, () => new T[arraySize], resetAction, null);
        }
    }
}