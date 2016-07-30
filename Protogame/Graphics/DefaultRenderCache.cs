namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Graphics;
    
    public class DefaultRenderCache : IRenderCache
    {
        private readonly Dictionary<string, IndexBuffer> m_IndexBuffers;
        
        private readonly Dictionary<string, VertexBuffer> m_VertexBuffers;
        
        public DefaultRenderCache()
        {
            this.m_VertexBuffers = new Dictionary<string, VertexBuffer>();
            this.m_IndexBuffers = new Dictionary<string, IndexBuffer>();
        }
        
        public void Free<T>(string name) where T : class
        {
            if (typeof(VertexBuffer).IsAssignableFrom(typeof(T)))
            {
                var buffer = this.m_VertexBuffers[name];
                buffer.Dispose();
                this.m_VertexBuffers.Remove(name);
            }
            else if (typeof(IndexBuffer).IsAssignableFrom(typeof(T)))
            {
                var buffer = this.m_IndexBuffers[name];
                buffer.Dispose();
                this.m_IndexBuffers.Remove(name);
            }
            else
            {
                throw new NotSupportedException("Unable to cache " + typeof(T).FullName + ".");
            }
        }
        
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
        
        public T GetOrSet<T>(string name, Func<T> setter) where T : class
        {
            if (this.Has<T>(name))
            {
                return this.Get<T>(name);
            }

            return this.Set(name, setter());
        }
        
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

        public int VertexBuffersCached => m_VertexBuffers.Count;
        public int IndexBuffersCached => m_IndexBuffers.Count;
    }
}