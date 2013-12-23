using System;
using System.Collections.Generic;

namespace Protogame
{
    public abstract class AIAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }

        public AIAsset(string name, string value)
        {
            this.Name = name;
        }

        public abstract void Update(IGameContext gameContext, IUpdateContext updateContext);
        public abstract void Render(IGameContext gameContext, IRenderContext renderContext);

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

