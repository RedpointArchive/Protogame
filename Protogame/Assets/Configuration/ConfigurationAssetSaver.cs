#if FALSE

using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    /// <summary>
    /// The configuration asset saver.
    /// </summary>
    public class ConfigurationAssetSaver : IAssetSaver
    {
        /// <summary>
        /// The can handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanHandle(IAsset asset)
        {
            return asset is ConfigurationAsset;
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="dynamic"/>.
        /// </returns>
        public IRawAsset Handle(IAsset asset, AssetTarget target)
        {
            var configurationAsset = asset as ConfigurationAsset;
            var settings = configurationAsset.Settings;

            return new DictionaryBasedRawAsset(FlattenDictionary(settings));
        }

        // TODO: Move this into a service instead of static implementation.
        public static Dictionary<string, object> FlattenDictionary(Dictionary<string, Dictionary<string, object>> settings)
        {
            var dict = new Dictionary<string, object>();
            dict["Loader"] = typeof(ConfigurationAssetLoader).FullName;
            dict["GroupCount"] = settings.Count;
            var groupNames = settings.Keys.ToList();
            for (var i = 0; i < groupNames.Count; i++)
            {
                var groupName = groupNames[i];
                var gi = i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                dict["GroupName" + gi] = groupName;
                dict["GroupKeyCount" + gi] = settings[groupName].Count;

                var groupKeys = settings[groupName].Keys.ToList();
                for (var k = 0; k < groupKeys.Count; k++)
                {
                    var groupKey = groupKeys[k];
                    var gk = k.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
                    var val = settings[groupName][groupKey];
                    dict["GroupKey" + gi + "KeyName" + gk] = groupKey;
                    dict["GroupKey" + gi + "KeyType" + gk] = val == null ? string.Empty : val.GetType().FullName;
                    dict["GroupKey" + gi + "KeyValue" + gk] = val;
                }
            }
            return dict;
        }
    }
}

#endif