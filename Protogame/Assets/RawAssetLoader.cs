//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Text;

namespace Protogame
{
    public class RawAssetLoader : IRawAssetLoader
    {
        public object LoadRawAsset(string name)
        {
            try
            {
                var entryAssemblyFileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
                var assetDirectory = new DirectoryInfo(
                     Path.Combine(entryAssemblyFileInfo.Directory.FullName, "Content"));
                if (!assetDirectory.Exists)
                    assetDirectory.Create();
                var file = new FileInfo(
                    Path.Combine(
                        assetDirectory.FullName,
                        name.Replace('.', Path.DirectorySeparatorChar) + ".asset"));
                using (var reader = new StreamReader(file.FullName, Encoding.UTF8))
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                    return serializer.Deserialize<object>(reader.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                throw new AssetNotFoundException(name, ex);
            }
        }
    }
}

