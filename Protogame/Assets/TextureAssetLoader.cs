//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;

namespace Protogame
{
    public class TextureAssetLoader : IAssetLoader
    {
        private IAssetContentManager m_AssetContentManager;
        
        public TextureAssetLoader(IAssetContentManager assetContentManager)
        {
            this.m_AssetContentManager = assetContentManager;
        }
    
        public bool CanHandle(dynamic data)
        {
            return data.Loader == typeof(TextureAssetLoader).FullName;
        }

        public IAsset Handle(string name, dynamic data)
        {
            return new TextureAsset(
                this.m_AssetContentManager,
                name,
                data.TextureData);
        }

        public IAsset GetDefault(string name)
        {
            throw new InvalidOperationException();
        }
    }
}

