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
        
        public bool CanHandle(IRawAsset data)
        {
            return typeof(AIAsset).IsAssignableFrom(data.GetProperty<Type>("Type"));
        }
        
        public bool CanNew()
        {
            return false;
        }
        
        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            throw new NotSupportedException();
        }
        
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            throw new NotSupportedException();
        }

        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
        {
            var value = (AIAsset)this.m_Kernel.Get(data.GetProperty<Type>("Type"));
            value.Name = name;
            return value;
        }
    }
}