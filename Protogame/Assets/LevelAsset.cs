using System;

namespace Protogame
{
    public class LevelAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public string Value { get; set; }
        public string SourcePath { get; set; }

        public LevelAsset(
            string name,
            string value,
            string sourcePath)
        {
            this.Name = name;
            this.Value = value;
            this.SourcePath = sourcePath;
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(LevelAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to LevelAsset.");
        }
    }
}

