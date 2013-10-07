using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Protogame;

namespace ProtogameAssetManager
{
    public class AssetManagerWorld : IWorld
    {
        public IAssetManager AssetManager { get; set; }
        public List<IEntity> Entities { get; set; }
        private DateTime m_Start;
        private AssetManagerLayout m_Layout;
        private static Dictionary<Type, IAssetEditor> m_Editors;
        private IAssetEditor m_CurrentEditor;
        private ISkin m_Skin;

        static AssetManagerWorld()
        {
            LoadEditorsForAssets();
        }
        
        public void Dispose()
        {
        }

        public static void LoadEditorsForAssets()
        {
            m_Editors = new Dictionary<Type, IAssetEditor>();
            foreach (var mapping in
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(IAssetEditor).IsAssignableFrom(type)
                where !type.IsInterface
                where !type.IsAbstract
                let a = Activator.CreateInstance(type) as IAssetEditor
                select new {
                    AssetType = a.GetAssetType(),
                    Editor = a })
            {
                m_Editors.Add(mapping.AssetType, mapping.Editor);
            }
        }

        public AssetManagerWorld(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities renderUtilities,
            ISkin skin,
            IAssetLoader[] loaders)
        {
            this.Entities = new List<IEntity>();
            this.m_Skin = skin;
            this.m_Start = DateTime.Now;

            // Add the asset manager layout.
            var entity = new CanvasEntity(this.m_Skin);
            this.m_Layout = new AssetManagerLayout(assetManagerProvider, renderUtilities, loaders, entity);
            entity.Canvas = this.m_Layout;
            this.Entities.Add(entity);

            this.m_Layout.MarkDirty.Click += (sender, e) =>
            {
                foreach (var asset in this.AssetManager.GetAll())
                    this.AssetManager.Dirty(asset.Name);
            };

            this.m_Layout.Bake.Click += (sender, e) =>
            {
                if (this.m_CurrentEditor != null)
                    this.m_CurrentEditor.Bake(this.AssetManager);
                var item = this.m_Layout.AssetTree.SelectedItem as AssetTreeItem;
                if (item == null)
                    return;
                this.AssetManager.Bake(item.Asset);
            };

            this.m_Layout.AssetTree.SelectedItemChanged += (sender, e) =>
            {
                if (this.m_CurrentEditor != null)
                    this.m_CurrentEditor.FinishLayout(this.m_Layout.EditorContainer, this.AssetManager);
                var item = this.m_Layout.AssetTree.SelectedItem as AssetTreeItem;
                if (item != null && m_Editors.ContainsKey(item.Asset.GetType()))
                {
                    this.m_CurrentEditor = m_Editors[item.Asset.GetType()];
                    this.m_CurrentEditor.SetAsset(item.Asset);
                    this.m_CurrentEditor.BuildLayout(this.m_Layout.EditorContainer, this.AssetManager);
                }
                else
                {
                    this.m_CurrentEditor = null;
                    this.m_Layout.EditorContainer.SetChild(
                        new Label { Text = "No editor for " + (item == null ? "folders" : item.Asset.GetType().Name) });
                }
            };
            
            this.m_Layout.ExitClick += (sender, e) => 
            {
                Environment.Exit(0);
            };
            
            this.m_Layout.BakeAllClick += (sender, e) => 
            {
                foreach (var asset in this.AssetManager.GetAll())
                    this.AssetManager.Bake(asset);
            };
            
            this.m_Layout.CreateNameEntered += (sender, e) => 
            {
                var asset = e.Loader.GetNew(this.AssetManager, this.m_Layout.PromptName.Text);
                assetManagerProvider.GetAssetManager(false).Bake(asset);
                this.m_Layout.AssetTree.AddChild(new AssetTreeItem
                {
                    Text = this.m_Layout.PromptName.Text,
                    Asset = asset
                });
            };
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public virtual void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var status = this.AssetManager.IsRemoting ?
                ("Connected for " +
                    (int)((DateTime.Now - this.m_Start).TotalSeconds) +
                    " seconds") :
                "Running Locally";
            status += " (" + gameContext.FPS + " FPS; " + (Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024) + "MB)";
            this.m_Layout.Status.Text = status;

            // Get the new state, and the items in the tree.
            var assets = this.AssetManager.GetAll();
            var existing = this.m_Layout.AssetTree.Children.Cast<TreeItem>();

            // Find items we need to add.
            foreach (var @add in assets.Where(x => !existing.Where(y => y is AssetTreeItem)
                .Cast<AssetTreeItem>().Select(y => y.Asset).Contains(x)))
            {
                var dirtyMark = "";
                if (@add is NetworkAsset)
                    dirtyMark = (@add as NetworkAsset).IsDirty ? "*" : "";
                this.m_Layout.AssetTree.AddChild(new AssetTreeItem
                {
                    Text = @add.Name + dirtyMark,
                    Asset = @add.Resolve<IAsset>() // resolve any NetworkAssets
                });
            }

            // Find items we need to remove.
            foreach (var @remove in existing.Where(x => x is AssetTreeItem)
                .Cast<AssetTreeItem>().Where(x => !assets.Contains(x.Asset)))
            {
                this.m_Layout.AssetTree.RemoveChild(@remove);
            }
        }
    }
}
