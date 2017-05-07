namespace Protogame
{
    using System;
    
    public class UserInterfaceAsset : IAsset
    {
        public UserInterfaceAsset(string name, string userInterfaceData, string userInterfaceFormat)
        {
            Name = name;
            UserInterfaceData = userInterfaceData;
            UserInterfaceFormat = userInterfaceFormat;
        }
        
        public string Name { get; }
        
        public string UserInterfaceData { get; set; }
        
        public string UserInterfaceFormat { get; set; }
    }
}