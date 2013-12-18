using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class FontAsset : MarshalByRefObject, IAsset
    {
        private IAssetContentManager m_AssetContentManager;
        public string Name { get; private set; }
        public SpriteFont Font { get; private set; }
        public PlatformData PlatformData { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public bool UseKerning { get; set; }
        public int Spacing { get; set; }

        public FontAsset(
            IAssetContentManager assetContentManager,
            string name,
            string fontName,
            int fontSize,
            bool useKerning,
            int spacing,
            PlatformData data)
        {
            this.Name = name;
            this.FontName = fontName;
            this.FontSize = fontSize;
            this.UseKerning = useKerning;
            this.Spacing = spacing;
            this.PlatformData = data;
            this.m_AssetContentManager = assetContentManager;

            if (this.PlatformData != null)
            {
                this.ReloadFont();
            }
        }

        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return this.FontName == null;
            }
        }
        
        public void ReloadFont()
        {
            if (this.m_AssetContentManager != null && this.PlatformData != null)
            {
                using (var stream = new MemoryStream(this.PlatformData.Data))
                {
                    this.m_AssetContentManager.SetStream(this.Name, stream);
                    this.m_AssetContentManager.Purge(this.Name);
                    this.Font = this.m_AssetContentManager.Load<SpriteFont>(this.Name);
                    this.m_AssetContentManager.UnsetStream(this.Name);
                }
            }
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(FontAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to FontAsset.");
        }
    }
}

