using System;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface IRenderAutoCache
    {
        VertexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, Func<VertexBuffer> vertexFactory);

        VertexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, int frameExpiry, Func<VertexBuffer> vertexFactory);

        IndexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, Func<IndexBuffer> indexFactory);

        IndexBuffer AutoCache(string name, object[] parameters, IGameContext gameContext, int frameExpiry, Func<IndexBuffer> indexFactory);

        void Update(IGameContext gameContext, IUpdateContext updateContext);

        int VertexBuffersCached { get; }

        int IndexBuffersCached { get; }
    }
}