using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Protogame
{
    public class EmbeddedSourceLoadStrategy : ILoadStrategy
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
                return new[] { "asset" };
            }
        }

        public object AttemptLoad(string path, string name)
        {
            var embedded = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            where !assembly.IsDynamic
                            from resource in assembly.GetManifestResourceNames()
                            where resource == assembly.GetName().Name + "." + name + ".asset"
                            select assembly.GetManifestResourceStream(resource)).ToList();
            if (embedded.Any())
            {
                using (var reader = new StreamReader(embedded.First(), Encoding.UTF8))
                {
                    return JsonConvert.DeserializeObject<dynamic>(reader.ReadToEnd());
                }
            }
            return null;
        }
    }
}
