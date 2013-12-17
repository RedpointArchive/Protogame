using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Protogame
{
    public class EffectAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public byte[] GLEffectData { get; private set; }
        public byte[] DXEffectData { get; private set; }
        public string SourcePath { get; private set; }
        public Effect Effect { get; private set; }

        public EffectAsset(
            string name,
            string sourcePath,
            byte[] glEffectData,
            byte[] dxEffectData)
        {
            this.Name = name;
            this.SourcePath = sourcePath;
            this.GLEffectData = glEffectData;
            this.DXEffectData = dxEffectData;
        }
        
        public void LoadEffect(GraphicsDevice device)
        {
#if WINDOWS
            if (this.DXEffectData != null)
            {
                this.Effect = new Effect(device, this.DXEffectData);
                if (this.Effect == null)
                    throw new InvalidOperationException("Unable to load effect.");
            }
#else
            if (this.GLEffectData != null)
            {
                this.Effect = new Effect(device, this.GLEffectData);
                if (this.Effect == null)
                    throw new InvalidOperationException("Unable to load effect.");
            }
#endif
            return this.Effect;
        }
        
        public bool CompileEffect(out string message)
        {
#if !WINDOWS
            message = "You must be running Windows to recompile effects";
            return false;
#endif
        
            var currentAssemblyPath = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var twomgfxPath = Path.Combine(currentAssemblyPath.Directory.FullName, "2MGFX.exe");
            
            if (!File.Exists(twomgfxPath))
            {
                message = "Unable to locate 2MGFX.exe";
                return false;
            }
            
            message = "Execution of 2MGFX not implemented";
            return false;
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(EffectAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to EffectAsset.");
        }
    }
}

