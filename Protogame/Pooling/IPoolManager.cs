namespace Protogame
{
    using System;

    public interface IPoolManager
    {
        IPool<T> NewPool<T>(string name, int size, Action<T> resetAction) where T : class, new();

        IPool<T> NewPool<T>(string name, int size, Func<T> factoryFunc, Action<T> resetAction) where T : class;

        IPool<T> NewPool<T>(string name, int size, Func<T> factoryFunc, Action<T> resetAction, Action<T> newAction) where T : class;

        IPool<T> NewScalingPool<T>(string name, int increment, Action<T> resetAction) where T : class, new();

        IPool<T> NewScalingPool<T>(string name, int increment, Func<T> factoryFunc, Action<T> resetAction) where T : class;

        IPool<T> NewScalingPool<T>(string name, int increment, Func<T> factoryFunc, Action<T> resetAction, Action<T> newAction) where T : class;

        IPool<T[]> NewArrayPool<T>(string name, int size, int arraySize, Action<T[]> resetAction);

        IPool<T[]> NewScalingArrayPool<T>(string name, int increment, int arraySize, Action<T[]> resetAction);
    }
}