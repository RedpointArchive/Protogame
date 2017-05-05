#if FALSE

namespace Protogame
{
    public class LogicControlScriptAssetLoader : IAssetLoader
    {
        public bool CanLoad(IRawAsset data)
        {
            return data.GetProperty<string>("Loader") == typeof(LogicControlScriptAssetLoader).FullName;
        }

        public IAsset Load(string name, IRawAsset data)
        {
            return new ScriptAsset(
                name,
                data.GetProperty<string>("Code"),
                new LogicControlScriptEngine(data.GetProperty<string>("Code")));
        }
    }
}

#endif