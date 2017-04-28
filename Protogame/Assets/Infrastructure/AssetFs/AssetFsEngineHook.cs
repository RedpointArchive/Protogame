namespace Protogame
{
    public class AssetFsEngineHook : IEngineHook
    {
        private readonly IAssetFs _assetFs;

        public AssetFsEngineHook(IAssetFs assetFs)
        {
            _assetFs = assetFs;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _assetFs.Update();
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _assetFs.Update();
        }
    }
}
