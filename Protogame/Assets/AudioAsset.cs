using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class AudioAsset : MarshalByRefObject, IAsset
    {
        //private IAssetContentManager m_AssetContentManager;
        //private IContentCompiler m_ContentCompiler;
        public string Name { get; private set; }
        public SoundEffect Audio { get; private set; }
        public byte[] Data { get; set; }
        public string SourcePath { get; set; }

        public AudioAsset(
            //IContentCompiler contentCompiler,
            //IAssetContentManager assetContentManager,
            string name,
            string sourcePath,
            byte[] data)
        {
            this.Name = name;
            this.SourcePath = sourcePath;
            this.Data = data;
            //this.m_AssetContentManager = assetContentManager;
            //this.m_ContentCompiler = contentCompiler;
            
            this.ReloadAudio();
        }
        
        public void ReloadAudio()
        {
            if (/*this.m_AssetContentManager != null && */this.Data != null)
            {
                using (var stream = new MemoryStream(this.Data))
                {
                    this.Audio = SoundEffect.FromStream(stream);
                    if (this.Audio == null)
                        throw new InvalidOperationException("Unable to load effect from stream.");
                    //this.m_AssetContentManager.SetStream(this.Name, stream);
                    //this.m_AssetContentManager.Purge(this.Name);
                    //this.Audio = this.m_AssetContentManager.Load<SoundEffect>(this.Name);
                    //this.m_AssetContentManager.UnsetStream(this.Name);
                }
            }
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(AudioAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to AudioAsset.");
        }
    }
}

