//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Protogame
{
    public class FontAsset : MarshalByRefObject, IAsset
    {
        private IAssetContentManager m_AssetContentManager;
        private IContentCompiler m_ContentCompiler;
        public string Name { get; private set; }
        public SpriteFont Font { get; private set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public byte[] FontData { get; set; }

        public FontAsset(
            IContentCompiler contentCompiler,
            IAssetContentManager assetContentManager,
            string name,
            string fontName,
            int fontSize,
            byte[] fontData)
        {
            this.Name = name;
            this.FontName = fontName;
            this.FontSize = fontSize;
            this.FontData = fontData;
            this.m_AssetContentManager = assetContentManager;
            this.m_ContentCompiler = contentCompiler;
            
            this.ReloadFont();
        }
        
        public void ReloadFont()
        {
            if (this.m_AssetContentManager != null && this.FontData != null)
            {
                using (var stream = new MemoryStream(this.FontData))
                {
                    this.m_AssetContentManager.SetStream(this.Name, stream);
                    this.m_AssetContentManager.Purge(this.Name);
                    this.Font = this.m_AssetContentManager.Load<SpriteFont>(this.Name);
                    this.m_AssetContentManager.UnsetStream(this.Name);
                }
            }
        }
        
        public void RebuildFont()
        {
            if (string.IsNullOrWhiteSpace(this.FontName))
                return;
            if (this.FontSize <= 0)
                return;
            var data = this.m_ContentCompiler.BuildSpriteFont(
                this.FontName,
                this.FontSize,
                0,
                true);
            if (data != null)
            {
                this.FontData = data;
                this.ReloadFont();
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

