using System;
using Protogame;
using System.Collections.Generic;
using System.Linq;

namespace ProtogameAssetManager
{
    public class AssetManagerLayout : Canvas
    {
        public Label Status { get; private set; }
        public Button Bake { get; private set; }
        public Button MarkDirty { get; private set; }
        public MainMenu MainMenu { get; private set; }
        public TreeView AssetTree { get; private set; }
        public SingleContainer EditorContainer { get; private set; }

        public AssetManagerLayout(
            IAssetManagerProvider assetManagerProvider,
            IRenderUtilities renderUtilities,
            IEnumerable<IAssetLoader> loaders)
        {
            var toolbarContainer = new HorizontalContainer();
            toolbarContainer.AddChild(new SingleContainer(), "*");
            toolbarContainer.AddChild(this.Bake = new Button { Text = "Bake" }, "50");
            toolbarContainer.AddChild(this.MarkDirty = new Button { Text = "Mark Dirty" }, "80");

            var assetContainer = new VerticalContainer();
            assetContainer.AddChild(toolbarContainer, "20");
            assetContainer.AddChild(this.EditorContainer = new SingleContainer(), "*");

            var contentContainer = new HorizontalContainer();
            contentContainer.AddChild(this.AssetTree = new TreeView(), "50%");
            contentContainer.AddChild(assetContainer, "50%");

            var menuContainer = new VerticalContainer();
            menuContainer.AddChild(this.MainMenu = new MainMenu(assetManagerProvider, renderUtilities), "24");
            menuContainer.AddChild(contentContainer, "*");
            menuContainer.AddChild(this.Status = new Label { Text = "..." }, "24");
            this.SetChild(menuContainer);

            var exitItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = "Exit" };
            var assetManagerMenuItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = "Asset Manager" };
            exitItem.Click += (sender, e) =>
            {
                Environment.Exit(0);
            };
            assetManagerMenuItem.AddChild(exitItem);
            this.MainMenu.AddChild(assetManagerMenuItem);
            
            var newAssetMenuItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = "Create New..." };
            foreach (var loader in loaders.Where(x => x.CanNew()))
            {
                var createNewMenuItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = loader.GetType().Name };
                createNewMenuItem.Click += (sender, e) =>
                {
                    var asset = loader.GetNew("test");
                    this.AssetTree.AddChild(new AssetTreeItem
                    {
                        Text = "test",
                        Asset = asset
                    });
                };
                newAssetMenuItem.AddChild(createNewMenuItem);
            }
            this.MainMenu.AddChild(newAssetMenuItem);
        }
    }
}

