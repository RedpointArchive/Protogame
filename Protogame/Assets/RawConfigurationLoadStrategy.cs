using System.Collections.Generic;

namespace Protogame
{
    using System;
    using System.IO;

    /// <summary>
    /// The strategy for loading configuration from INI files.
    /// </summary>
    public class RawConfigurationLoadStrategy : ILoadStrategy
    {
        /// <summary>
        /// Gets the asset extensions.
        /// </summary>
        /// <value>
        /// The asset extensions.
        /// </value>
        public string[] AssetExtensions
        {
            get
            {
                return new[] { "ini" };
            }
        }

        public bool ScanSourcePath
        {
            get
            {
                return true;
            }
        }

        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified, bool noTranslate = false)
        {
            var file = new FileInfo(Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".ini"));
            if (file.Exists)
            {
                lastModified = file.LastWriteTime;
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        var currentGroupName = string.Empty;
                        var dict = new Dictionary<string, Dictionary<string, object>>();

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();

                            if (string.IsNullOrWhiteSpace(line.Trim()))
                            {
                                // Do nothing.
                            }
                            else if (line.Trim().StartsWith(";", StringComparison.InvariantCulture))
                            {
                                // Do nothing (comment).
                            }
                            else if (line.Trim().StartsWith("[", StringComparison.InvariantCulture))
                            {
                                // Start of INI file group.
                                currentGroupName = line.Trim().Trim(new[] { '[', ']' });
                            }
                            else if (line.Contains("="))
                            {
                                var split = line.Trim().Split(new[] { '=' }, 2);
                                var key = split[0];
                                var value = split[1];

                                long longValue;
                                double doubleValue;
                                if (long.TryParse(value, out longValue))
                                {
                                    dict[currentGroupName][key] = longValue;
                                }
                                else if (double.TryParse(value, out doubleValue))
                                {
                                    dict[currentGroupName][key] = doubleValue;
                                }
                                else
                                {
                                    dict[currentGroupName][key] = value;
                                }
                            }
                        }

                        return new DictionaryBasedRawAsset(ConfigurationAssetSaver.FlattenDictionary(dict));
                    }
                }
            }

            return null;
        }
    }
}