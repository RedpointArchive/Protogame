using System;

namespace Protogame
{    
    public class LevelAsset : IAsset
    {
        public LevelAsset(string name, string levelData, string levelDataFormat)
        {
            Name = name;
            LevelData = levelData;
            LevelDataFormat = levelDataFormat;
        }
        
        public string Name { get; }

        public string LevelData { get; }
        
        public string LevelDataFormat { get; }
    }
}