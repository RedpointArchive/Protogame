using Ninject;
using Protogame;
using System.Diagnostics;
using System;

namespace ProtogameAssetManager
{
    public class AssetManagerGame : CoreGame<AssetManagerWorld, DefaultWorldManager>
    {
        private IAssetManager m_AssetManager;

        private AssetManagerWorld AssetWorld
        {
            get { return this.GameContext.World as AssetManagerWorld; }
        }

        public AssetManagerGame(
            IKernel kernel,
            IAssetManager assetManager) : base(kernel)
        {
            this.m_AssetManager = assetManager;
        }
        
        protected override void LoadContent()
        {
            base.LoadContent();

            this.GameContext.ResizeWindow(420, 800);
            this.IsMouseVisible = true;
            this.Window.Title = "Tychaia Asset Manager";

            this.m_AssetManager.Status = "Initializing...";
            this.AssetWorld.AssetManager = this.m_AssetManager;
        }
    }
}
