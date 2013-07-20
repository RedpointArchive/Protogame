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
    public class RawAssetSaver : IRawAssetSaver
    {
        public void SaveRawAsset(string name, object data)
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
                this.CreateDirectories(file.Directory);
                using (var writer = new StreamWriter(file.FullName, false, Encoding.UTF8))
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.RegisterConverters(new[] { new DynamicJsonUnconverter() });
                    writer.Write(serializer.Serialize(data));
                }
            }
            catch (Exception ex)
            {
                throw new AssetNotFoundException(name, ex);
            }
        }
        
        private void CreateDirectories(DirectoryInfo directory)
        {
            if (directory.Exists)
                return;
            this.CreateDirectories(directory.Parent);
            directory.Create();
        }
    }
}

