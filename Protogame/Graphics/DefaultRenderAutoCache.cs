using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultRenderAutoCache : IRenderAutoCache
    {
        private readonly IRenderCache _renderCache;
        private readonly Dictionary<string, int> _frameExpiryTracker;
        private int _vertexBufferCount;
        private int _indexBufferCount;

        public DefaultRenderAutoCache(IRenderCache renderCache)
        {
            _renderCache = renderCache;
            _frameExpiryTracker = new Dictionary<string, int>();
        }

        public VertexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, Func<VertexBuffer> vertexFactory)
        {
            var key = "vb:" + FormKey(name, parameters);
            _frameExpiryTracker[key] = gameContext.FrameCount + 300;
            if (_renderCache.Has<VertexBuffer>(key))
            {
                return _renderCache.Get<VertexBuffer>(key);
            }
            _vertexBufferCount++;
            return _renderCache.Set<VertexBuffer>(key, vertexFactory());
        }

        public VertexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, int frameExpiry, Func<VertexBuffer> vertexFactory)
        {
            var key = "vb:" + FormKey(name, parameters);
            _frameExpiryTracker[key] = gameContext.FrameCount + frameExpiry;
            if (_renderCache.Has<VertexBuffer>(key))
            {
                return _renderCache.Get<VertexBuffer>(key);
            }
            _vertexBufferCount++;
            return _renderCache.Set<VertexBuffer>(key, vertexFactory());
        }

        public IndexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, Func<IndexBuffer> indexFactory)
        {
            var key = "ib:" + FormKey(name, parameters);
            _frameExpiryTracker[key] = gameContext.FrameCount + 300;
            if (_renderCache.Has<IndexBuffer>(key))
            {
                return _renderCache.Get<IndexBuffer>(key);
            }
            _indexBufferCount++;
            return _renderCache.Set<IndexBuffer>(key, indexFactory());
        }

        public IndexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, int frameExpiry, Func<IndexBuffer> indexFactory)
        {
            var key = "ib:" + FormKey(name, parameters);
            _frameExpiryTracker[key] = gameContext.FrameCount + frameExpiry;
            if (_renderCache.Has<IndexBuffer>(key))
            {
                return _renderCache.Get<IndexBuffer>(key);
            }
            _indexBufferCount++;
            return _renderCache.Set<IndexBuffer>(key, indexFactory());
        }

        private string FormKey(string name, object[] parameters)
        {
            var n = name;
            if (parameters == null || parameters.Length == 0)
            {
                return n;
            }
            for (var i = 0; i < parameters.Length; i++)
            {
                n += ":" + (parameters[i]?.GetHashCode() ?? 0);
            }
            return n;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            foreach (var k in _frameExpiryTracker.Keys.ToArray())
            {
                if (_frameExpiryTracker[k] < gameContext.FrameCount)
                {
                    if (k.StartsWith("vb:"))
                    {
                        _renderCache.Free<VertexBuffer>(k);
                        _vertexBufferCount--;
                    }
                    else
                    {
                        _renderCache.Free<IndexBuffer>(k);
                        _indexBufferCount--;
                    }

                    _frameExpiryTracker.Remove(k);
                }
            }
        }

        public int VertexBuffersCached => _vertexBufferCount;
        public int IndexBuffersCached => _indexBufferCount;
    }
}
