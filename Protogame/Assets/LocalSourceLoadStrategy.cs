using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Protogame
{
    public class LocalSourceLoadStrategy : ILoadStrategy
    {
        public bool ScanSourcePath { get { return true; } }

        public object AttemptLoad(string path, string name)
        {
            var file = new FileInfo(
                Path.Combine(
                    path,
                    name.Replace('.', Path.DirectorySeparatorChar) + ".asset"));
            if (file.Exists)
            {
                using (var reader = new StreamReader(file.FullName, Encoding.UTF8))
                {
                    return JsonConvert.DeserializeObject<dynamic>(reader.ReadToEnd());
                }
            }
            return null;
        }
    }
}
