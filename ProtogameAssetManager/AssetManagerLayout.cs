using System;
using Protogame;
using System.Collections.Generic;
using System.Linq;

namespace ProtogameAssetManager
{
    public class AssetManagerLayout : Canvas
    {
        private CanvasEntity m_CanvasEntity;
    
        public Label Status { get; private set; }
        public Button Bake { get; private set; }
        public Button MarkDirty { get; private set; }
        public MainMenu MainMenu { get; private set; }
        public TreeView AssetTree { get; private set; }
        public SingleContainer EditorContainer { get; private set; }
        public Window PromptWindow { get; private set; }
        public TextBox PromptName { get; private set; }

        public event EventHandler ExitClick;
        public event EventHandler BakeAllClick;
        public event EventHandler<CreateEventArgs> CreateNameEntered;
        
        public class CreateEventArgs : EventArgs
        {
            public IAssetLoader Loader { get; private set; }
            
            public CreateEventArgs(IAssetLoader loader)
            {
                this.Loader = loader;
            }
        }

        public AssetManagerLayout(
            IAssetManagerProvider assetManagerProvider,
            IRenderUtilities renderUtilities,
            IEnumerable<IAssetLoader> loaders,
            CanvasEntity canvasEntity)
        {
            this.m_CanvasEntity = canvasEntity;
        
            var toolbarContainer = new HorizontalContainer();
            toolbarContainer.AddChild(new SingleContainer(), "*");
            toolbarContainer.AddChild(this.Bake = new Button { Text = "Bake" }, "50");
            toolbarContainer.AddChild(this.MarkDirty = new Button { Text = "Mark Dirty" }, "80");

            var assetContainer = new VerticalContainer();
            assetContainer.AddChild(toolbarContainer, "20");
            assetContainer.AddChild(this.EditorContainer = new SingleContainer(), "*");

            var contentContainer = new HorizontalContainer();
            contentContainer.AddChild(this.AssetTree = new TreeView(), "210");
            contentContainer.AddChild(assetContainer, "*");

            var menuContainer = new VerticalContainer();
            menuContainer.AddChild(this.MainMenu = new MainMenu(assetManagerProvider, renderUtilities), "24");
            menuContainer.AddChild(contentContainer, "*");
            menuContainer.AddChild(this.Status = new Label { Text = "..." }, "24");
            this.SetChild(menuContainer);

            var exitItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = "Exit" };
            var bakeAllItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = "Bake All" };
            var assetManagerMenuItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = "Asset Manager" };
            exitItem.Click += (sender, e) =>
            {
                if (this.ExitClick != null)
                    this.ExitClick(sender, e);
            };
            bakeAllItem.Click += (sender, e) =>
            {
                if (this.BakeAllClick != null)
                    this.BakeAllClick(sender, e);
                (bakeAllItem.Parent as MenuItem).Active = false;
            };
            assetManagerMenuItem.AddChild(bakeAllItem);
            assetManagerMenuItem.AddChild(exitItem);
            this.MainMenu.AddChild(assetManagerMenuItem);
            
            var newAssetMenuItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = "Create New..." };
            foreach (var loader in loaders.Where(x => x.CanNew()))
            {
                var createNewMenuItem = new MenuItem(assetManagerProvider, renderUtilities) { Text = loader.GetType().Name };
                createNewMenuItem.Click += (sender, e) =>
                {
                    if (this.PromptWindow == null)
                    {
                        this.PromptForCreation(loader.GetType().Name, (_, _2) =>
                        {
                            if (this.CreateNameEntered != null)
                                this.CreateNameEntered(this, new CreateEventArgs(loader));
                        });
                    }
                };
                newAssetMenuItem.AddChild(createNewMenuItem);
            }
            this.MainMenu.AddChild(newAssetMenuItem);
        }
        
        public void PromptForCreation(string createType, EventHandler submit)
        {
            this.PromptName = new TextBox();
            
            var label = new Label();
            label.Text = "Enter the name of the new " + createType + ":";
            
            var form = new Form();
            form.AddControl("Name: ", this.PromptName);
            
            var submitButton = new Button();
            submitButton.Text = "Create";
            submitButton.Click += (sender, e) =>
            {
                submit(sender, e);
                this.m_CanvasEntity.Windows.Remove(this.PromptWindow);
                this.PromptWindow = null;
                this.PromptName = null;
            };
            
            var cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Click += (sender, e) =>
            {
                this.m_CanvasEntity.Windows.Remove(this.PromptWindow);
                this.PromptWindow = null;
                this.PromptName = null;
            };
            
            var horizontalContainer = new HorizontalContainer();
            horizontalContainer.AddChild(submitButton, "50%");
            horizontalContainer.AddChild(cancelButton, "50%");
            
            var verticalContainer = new VerticalContainer();
            verticalContainer.AddChild(label, "24");
            verticalContainer.AddChild(form, "*");
            verticalContainer.AddChild(horizontalContainer, "24");
            
            this.PromptWindow = new Window();
            this.PromptWindow.Modal = true;
            this.PromptWindow.Bounds = new Microsoft.Xna.Framework.Rectangle(
                (int)this.m_CanvasEntity.Width / 2 - 150,
                (int)this.m_CanvasEntity.Height / 2 - 75,
                300,
                150);
            this.PromptWindow.SetChild(verticalContainer);
            this.m_CanvasEntity.Windows.Add(this.PromptWindow);
            
            this.MainMenu.Active = false;
            foreach (var item in this.MainMenu.Children.Cast<MenuItem>())
                item.Active = false;
        }
    }
}

