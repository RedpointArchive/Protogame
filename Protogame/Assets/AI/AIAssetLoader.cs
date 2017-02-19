namespace Protogame
{
    using System;
    using Protoinject;
    
    public class AIAssetLoader : IAssetLoader
    {
        private readonly IKernel m_Kernel;
        
        public AIAssetLoader(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public bool CanLoad(IRawAsset data)
        {
            return typeof(AIAsset).IsAssignableFrom(data.GetProperty<Type>("Type"));
        }

        public IAsset Load(string name, IRawAsset data)
        {
            var value = (AIAsset)this.m_Kernel.Get(data.GetProperty<Type>("Type"));
            value.Name = name;
            return value;
        }
    }
}