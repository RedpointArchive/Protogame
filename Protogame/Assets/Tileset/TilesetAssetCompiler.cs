using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Protogame
{
    public class TilesetAssetCompiler : IAssetCompiler
    {
        public string[] Extensions => new[] { "tileset" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, IWritableSerializedAsset output)
        {
            TilesetJson tilesetJson;
            using (var stream = new StreamReader(await assetFile.GetContentStream().ConfigureAwait(false)))
            {
                tilesetJson = JsonConvert.DeserializeObject<TilesetJson>(await stream.ReadToEndAsync().ConfigureAwait(false));
            }

            output.SetLoader<IAssetLoader<TilesetAsset>>();
            output.SetString("TextureName", tilesetJson.TextureName);
            output.SetInt32("CellWidth", tilesetJson.CellWidth);
            output.SetInt32("CellHeight", tilesetJson.CellHeight);
        }

        private class TilesetJson
        {
            [JsonProperty("textureName")]
            public string TextureName { get; set; }

            [JsonProperty("cellWidth")]
            public int CellWidth { get; set; }
            
            [JsonProperty("cellHeight")]
            public int CellHeight { get; set; }
        }
    }
}
