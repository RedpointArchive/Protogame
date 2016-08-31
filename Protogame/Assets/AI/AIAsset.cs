namespace Protogame
{
    using System;
    
    public abstract class AIAsset : MarshalByRefObject, IAsset
    {
        public bool CompiledOnly => false;
        
        public string Name { get; set; }
        
        public bool SourceOnly => false;
        
        public abstract void Render(IEntity entity, IGameContext gameContext, IRenderContext renderContext);
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(AIAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to AIAsset.");
        }
        
        public abstract void Update(IEntity entity, IGameContext gameContext, IUpdateContext updateContext);
        
        public abstract void Update(IServerEntity entity, IServerContext serverContext, IUpdateContext updateContext);
    }
}