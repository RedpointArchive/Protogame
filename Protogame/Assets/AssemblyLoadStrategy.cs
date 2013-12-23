namespace Protogame
{
using System;
using System.Linq;
    
    /// <summary>
    /// A load strategy that supports loading classes as runtime assets.
    /// </summary>
    public class AssemblyLoadStrategy : ILoadStrategy
    {
        public bool ScanSourcePath
        {
            get
            {
                return false;
            }
        }
        
        public string[] AssetExtensions
        {
            get
            {
                return new string[0];
            }
        }

        public object AttemptLoad(string path, string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    if (typeof(IAsset).IsAssignableFrom(type))
                    {
                        var attribute = type.GetCustomAttributes(false).Cast<Attribute>().OfType<AssetAttribute>().FirstOrDefault();
                        if (attribute != null)
                            if (attribute.Name == name)
                                return new { Type = type, Name = name };
                    }

            return null;
        }
    }
}
