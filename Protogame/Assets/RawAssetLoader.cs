using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

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
                if (file.Exists)
                {
                    using (var reader = new StreamReader(file.FullName, Encoding.UTF8))
                    {
                        var serializer = new JavaScriptSerializer();
                        serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                        return serializer.Deserialize<object>(reader.ReadToEnd());
                    }
                }
                var embedded = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                where !assembly.IsDynamic
                                from resource in assembly.GetManifestResourceNames()
                                where resource == assembly.GetName().Name + "." + name + ".asset"
                                select assembly.GetManifestResourceStream(resource)).ToList();
                if (embedded.Any())
                {
                    using (var reader = new StreamReader(embedded.First(), Encoding.UTF8))
                    {
                        var serializer = new JavaScriptSerializer();
                        serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                        return serializer.Deserialize<object>(reader.ReadToEnd());
                    }
                }
                throw new AssetNotFoundException(name);
            }
            catch (Exception ex)
            {
                if (ex is AssetNotFoundException)
                    throw;
                throw new AssetNotFoundException(name, ex);
            }
        }
    }
}

