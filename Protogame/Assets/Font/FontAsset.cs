using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class FontAsset : IAsset, INativeAsset
    {
        private readonly IAssetContentManager _assetContentManager;
        private readonly byte[] _data;
        
        public FontAsset(
            IAssetContentManager assetContentManager, 
            string name, 
            string fontName, 
            float fontSize, 
            bool useKerning,
            float spacing, 
            byte[] data)
        {
            Name = name;
            FontName = fontName;
            FontSize = fontSize;
            UseKerning = useKerning;
            Spacing = spacing;
            _assetContentManager = assetContentManager;
            _data = data;
        }
        
        public SpriteFont Font { get; private set; }
        
        public string FontName { get; }
        
        public float FontSize { get; }
        
        public string Name { get; }
        
        public float Spacing { get; }
        
        public bool UseKerning { get; }

        public void ReadyOnGameThread()
        {
            if (_assetContentManager != null && _data != null)
            {
                using (var stream = new MemoryStream(_data))
                {
                    _assetContentManager.SetStream(Name, stream);
                    _assetContentManager.Purge(Name);
                    try
                    {
                        Font = _assetContentManager.Load<SpriteFont>(Name);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // This occurs when using assets without a game running, in which case we don't
                        // need the SpriteFont anyway (since we are probably just after the compiled asset).
                        if (ex.Message != "No Graphics Device Service")
                        {
                            throw;
                        }
                    }

                    _assetContentManager.UnsetStream(Name);
                }
            }
        }
    }
}