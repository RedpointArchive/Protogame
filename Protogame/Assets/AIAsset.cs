using System;
using System.Collections.Generic;

namespace Protogame
{
    public abstract class AIAsset : MarshalByRefObject, IAsset
    {
        public string Name
        {
            get;
            set;
        }
        
        public abstract void Update(IEntity entity, IGameContext gameContext, IUpdateContext updateContext);
        public abstract void Render(IEntity entity, IGameContext gameContext, IRenderContext renderContext);
        public abstract void Update(IServerEntity entity);

        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(AIAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to AIAsset.");
        }
    }
}

