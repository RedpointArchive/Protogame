namespace Protogame
{
    using System;

    /// <summary>
    /// The memory pool manager, which is used to create new pools
    /// of objects.
    /// </summary>
    /// <module>Pooling</module>
    public interface IPoolManager
    {
        /// <summary>
        /// Creates a new fixed-size memory pool of the specified size.
        /// <para>
        /// To use this method, the objects must have a public, parameter-less constructor.  If they
        /// do not, you should use one of the variants of this method call that require for a factory
        /// function.
        /// </para>
        /// </summary>
        /// <returns>The memory pool.</returns>
        /// <param name="name">The user-friendly name of the pool which can be viewed during memory debugging.</param>
        /// <param name="size">The size of the memory pool.</param>
        /// <param name="resetAction">An action which is called when the state of an object needs to be reset before reusing it.</param>
        /// <typeparam name="T">The type of objects to store in the memory pool.</typeparam>
        IPool<T> NewPool<T>(string name, int size, Action<T> resetAction) where T : class, new();

        /// <summary>
        /// Creates a new fixed-size memory pool of the specified size.
        /// </summary>
        /// <returns>The memory pool.</returns>
        /// <param name="name">The user-friendly name of the pool which can be viewed during memory debugging.</param>
        /// <param name="size">The size of the memory pool.</param>
        /// <param name="factoryFunc">The factory function which allocates new objects for the pool.</param>
        /// <param name="resetAction">An action which is called when the state of an object needs to be reset before reusing it.</param>
        /// <typeparam name="T">The type of objects to store in the memory pool.</typeparam>
        IPool<T> NewPool<T>(string name, int size, Func<T> factoryFunc, Action<T> resetAction) where T : class;

        /// <summary>
        /// Creates a new fixed-size memory pool of the specified size.
        /// </summary>
        /// <returns>The memory pool.</returns>
        /// <param name="name">The user-friendly name of the pool which can be viewed during memory debugging.</param>
        /// <param name="size">The size of the memory pool.</param>
        /// <param name="factoryFunc">The factory function which allocates new objects for the pool.</param>
        /// <param name="resetAction">An action which is called when the state of an object needs to be reset before reusing it.</param>
        /// <param name="newAction">An action which is called before an object is given out for the first time.</param>
        /// <typeparam name="T">The type of objects to store in the memory pool.</typeparam>
        IPool<T> NewPool<T>(string name, int size, Func<T> factoryFunc, Action<T> resetAction, Action<T> newAction) where T : class;

        /// <summary>
        /// Creates a new scaling memory pool which can allocate and de-allocate blocks of objects
        /// as needed.  This ensures that allocations of memory happen less frequently that they would
        /// otherwise for normal allocation and garbage collection.
        /// <para>
        /// To use this method, the objects must have a public, parameter-less constructor.  If they
        /// do not, you should use one of the variants of this method call that require for a factory
        /// function.
        /// </para>
        /// </summary>
        /// <returns>The memory pool.</returns>
        /// <param name="name">The user-friendly name of the pool which can be viewed during memory debugging.</param>
        /// <param name="increment">The size by which to increment the memory pool when more objects are needed, and the size by which to decrement the pool when less objects are needed.</param>
        /// <param name="resetAction">An action which is called when the state of an object needs to be reset before reusing it.</param>
        /// <typeparam name="T">The type of objects to store in the memory pool.</typeparam>
        IPool<T> NewScalingPool<T>(string name, int increment, Action<T> resetAction) where T : class, new();

        /// <summary>
        /// Creates a new scaling memory pool which can allocate and de-allocate blocks of objects
        /// as needed.  This ensures that allocations of memory happen less frequently that they would
        /// otherwise for normal allocation and garbage collection.
        /// <para>
        /// To use this method, the objects must have a public, parameter-less constructor.  If they
        /// do not, you should use one of the variants of this method call that require for a factory
        /// function.
        /// </para>
        /// </summary>
        /// <returns>The memory pool.</returns>
        /// <param name="name">The user-friendly name of the pool which can be viewed during memory debugging.</param>
        /// <param name="increment">The size by which to increment the memory pool when more objects are needed, and the size by which to decrement the pool when less objects are needed.</param>
        /// <param name="factoryFunc">The factory function which allocates new objects for the pool.</param>
        /// <param name="resetAction">An action which is called when the state of an object needs to be reset before reusing it.</param>
        /// <typeparam name="T">The type of objects to store in the memory pool.</typeparam>
        IPool<T> NewScalingPool<T>(string name, int increment, Func<T> factoryFunc, Action<T> resetAction) where T : class;

        /// <summary>
        /// Creates a new scaling memory pool which can allocate and de-allocate blocks of objects
        /// as needed.  This ensures that allocations of memory happen less frequently that they would
        /// otherwise for normal allocation and garbage collection.
        /// <para>
        /// To use this method, the objects must have a public, parameter-less constructor.  If they
        /// do not, you should use one of the variants of this method call that require for a factory
        /// function.
        /// </para>
        /// </summary>
        /// <returns>The memory pool.</returns>
        /// <param name="name">The user-friendly name of the pool which can be viewed during memory debugging.</param>
        /// <param name="increment">The size by which to increment the memory pool when more objects are needed, and the size by which to decrement the pool when less objects are needed.</param>
        /// <param name="factoryFunc">The factory function which allocates new objects for the pool.</param>
        /// <param name="resetAction">An action which is called when the state of an object needs to be reset before reusing it.</param>
        /// <param name="newAction">An action which is called before an object is given out for the first time.</param>
        /// <typeparam name="T">The type of objects to store in the memory pool.</typeparam>
        IPool<T> NewScalingPool<T>(string name, int increment, Func<T> factoryFunc, Action<T> resetAction, Action<T> newAction) where T : class;

        IPool<T[]> NewArrayPool<T>(string name, int size, int arraySize, Action<T[]> resetAction);

        IPool<T[]> NewScalingArrayPool<T>(string name, int increment, int arraySize, Action<T[]> resetAction);
    }
}