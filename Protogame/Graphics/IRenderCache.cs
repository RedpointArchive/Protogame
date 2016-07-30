namespace Protogame
{
    using System;

    /// <summary>
    /// The RenderCache interface.
    /// </summary>
    public interface IRenderCache
    {
        /// <summary>
        /// The free.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        void Free<T>(string name) where T : class;

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Get<T>(string name) where T : class;

        /// <summary>
        /// The get or set.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="setter">
        /// The setter.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetOrSet<T>(string name, Func<T> setter) where T : class;

        /// <summary>
        /// The has.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Has<T>(string name) where T : class;

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Set<T>(string name, T value) where T : class;
    }
}