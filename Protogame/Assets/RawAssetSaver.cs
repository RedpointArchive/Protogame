using System;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace Protogame
{
    public class RawAssetSaver : IRawAssetSaver
    {
        private string m_Path;
    
        public RawAssetSaver()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory).FullName;
            var contentDirectory = Path.Combine(directory, "Content");
            if (Directory.Exists(contentDirectory))
            {
                Directory.CreateDirectory(directory);
                directory = contentDirectory;
            }
            this.m_Path = directory;
        }
    
        public void SaveRawAsset(string name, object data)
        {
            try
            {
                var file = new FileInfo(
                    Path.Combine(
                        this.m_Path,
                        name.Replace('.', Path.DirectorySeparatorChar) + ".asset"));
                this.CreateDirectories(file.Directory);
                using (var writer = new StreamWriter(file.FullName, false, Encoding.UTF8))
                {
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;
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

