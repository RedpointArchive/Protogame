using System.Collections.Generic;

namespace Protogame
{
    /// <summary>
    /// The configuration asset loader.
    /// </summary>
    public class ConfigurationAssetLoader : IAssetLoader
    {
        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanHandle(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(ConfigurationAssetLoader).FullName;
        }

        /// <summary>
        /// The can new.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanNew()
        {
            return true;
        }

        /// <summary>
        /// The get default.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public IAsset GetDefault(IAssetManager assetManager, string name)
        {
            return null;
        }

        /// <summary>
        /// The get new.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public IAsset GetNew(IAssetManager assetManager, string name)
        {
            return new ConfigurationAsset(name, new Dictionary<string, Dictionary<string, object>>());
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="assetManager">
        /// The asset manager.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public IAsset Handle(IAssetManager assetManager, string name, IRawAsset data)
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