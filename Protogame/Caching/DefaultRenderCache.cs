namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The default render cache.
    /// </summary>
    public class DefaultRenderCache : IRenderCache
    {
        /// <summary>
        /// The m_ index buffers.
        /// </summary>
        private readonly Dictionary<string, IndexBuffer> m_IndexBuffers;

        /// <summary>
        /// The m_ vertex buffers.
        /// </summary>
        private readonly Dictionary<string, VertexBuffer> m_VertexBuffers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderCache"/> class.
        /// </summary>
        public DefaultRenderCache()
        {
            this.m_VertexBuffers = new Dictionary<string, VertexBuffer>();
            this.m_IndexBuffers = new Dictionary<string, IndexBuffer>();
        }

        /// <summary>
        /// The free.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public void Free<T>(string name) where T : class
        {
            if (typeof(VertexBuffer).IsAssignableFrom(typeof(T)))
            {
                var buffer = this.m_VertexBuffers[name];
                buffer.Dispose();
                this.m_VertexBuffers[name] = null;
            }
            else if (typeof(IndexBuffer).IsAssignableFrom(typeof(T)))
            {
                var buffer = this.m_IndexBuffers[name];
                buffer.Dispose();
                this.m_IndexBuffers[name] = null;
            }
            else
            {
                throw new NotSupportedException("Unable to cache " + typeof(T).FullName + ".");
            }
        }

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
        /// <exception cref="NotSupportedException">
        /// </exception>
        public T Get<T>(string name) where T : class
        {
            if (typeof(VertexBuffer).IsAssignableFrom(typeof(T)))
            {
                return this.m_VertexBuffers[name] as T;
            }

            if (typeof(IndexBuffer).IsAssignableFrom(typeof(T)))
            {
                return this.m_IndexBuffers[name] as T;
            }

            throw new NotSupportedException("Unable to cache " + typeof(T).FullName + ".");
        }

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
        public T GetOrSet<T>(string name, Func<T> setter) where T : class
        {
            if (this.Has<T>(name))
            {
                return this.Get<T>(name);
            }

            return this.Set(name, setter());
        }

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
        /// <exception cref="NotSupportedException">
        /// </exception>
        public bool Has<T>(string name) where T : class
        {
            if (typeof(VertexBuffer).IsAssignableFrom(typeof(T)))
            {
                return this.m_VertexBuffers.ContainsKey(name);
            }

            if (typeof(IndexBuffer).IsAssignableFrom(typeof(T)))
            {
                return this.m_IndexBuffers.ContainsKey(name);
            }

            throw new NotSupportedException("Unable to cache " + typeof(T).FullName + ".");
        }

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
        /// <exception cref="NotSupportedException">
        /// </exception>
        public T Set<T>(string name, T value) where T : class
        {
            if (this.Has<T>(name))
            {
                this.Free<T>(name);
            }

            if (typeof(VertexBuffer).IsAssignableFrom(typeof(T)))
            {
                this.m_VertexBuffers[name] = value as VertexBuffer;
            }
            else if (typeof(IndexBuffer).IsAssignableFrom(typeof(T)))
            {
                this.m_IndexBuffers[name] = value as IndexBuffer;
            }
            else
            {
                throw new NotSupportedException("Unable to cache " + typeof(T).FullName + ".");
            }

            return value;
        }
    }
}