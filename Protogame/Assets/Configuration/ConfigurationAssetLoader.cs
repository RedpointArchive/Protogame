#if FALSE

using System.Collections.Generic;

namespace Protogame
{
    public class ConfigurationAssetLoader : IAssetLoader
    {
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(ConfigurationAssetLoader).FullName;
        }
        
        public IAsset Load(string name, IRawAsset data)
        {
            var groupDict = new Dictionary<string, Dictionary<string, object>>();
            var groupCount = data.GetProperty<int>("GroupCount");
            for (var i = 0; i < groupCount; i++)
            {
                var gi = i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                var groupName = data.GetProperty<string>("GroupName" + gi);
                var keyCount = data.GetProperty<int>("GroupKeyCount" + gi);

                var dict = new Dictionary<string, object>();
                for (var k = 0; k < keyCount; k++)
                {
                    var gk = k.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                    var keyName = data.GetProperty<string>("GroupKey" + gi + "KeyName" + gk);
                    var keyType = data.GetProperty<string>("GroupKey" + gi + "KeyType" + gk);
                    object keyValue;

                    if (keyType == string.Empty)
                    {
                        keyValue = null;
                    }
                    else
                    {
                        keyValue = data.GetProperty(
                            System.Type.GetType(keyType),
                            "GroupKey" + gi + "KeyValue" + gk);
                    }

                    if (dict.ContainsKey(keyName))
                    {
                        dict.Remove(keyName);
                    }
                    dict.Add(keyName, keyValue);
                }

                if (groupDict.ContainsKey(groupName))
                {
                    groupDict.Remove(groupName);
                }
                groupDict.Add(groupName, dict);
            }

            return new ConfigurationAsset(name, groupDict);
        }
    }
}

#endif