using System;
using Protogame;

namespace ProtogameAssetManager
{
    public abstract class AssetEditor<T> : IAssetEditor
    {
        protected T m_Asset;

        public void SetAsset(IAsset asset)
        {
            this.m_Asset = (T)asset;
        }

        public Type GetAssetType()
        {
            return typeof(T);
        }

        public abstract void BuildLayout(SingleContainer editorContainer, IAssetManager assetManager);

        public virtual void FinishLayout(SingleContainer editorContainer, IAssetManager assetManager)
        {
        }

        public virtual void Bake(IAssetManager assetManager)
        {
        }
    }
}

