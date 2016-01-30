namespace Protogame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The basic skin, but with containers set to transparent.
    /// </summary>
    public class TransparentBasicSkin : ISkin
    {
        /// <summary>
        /// The m_ asset manager.
        /// </summary>
        private readonly IAssetManager m_AssetManager;

        /// <summary>
        /// The m_ basic skin.
        /// </summary>
        private readonly IBasicSkin m_BasicSkin;

        /// <summary>
        /// The m_ render utilities.
        /// </summary>
        private readonly I2DRenderUtilities m_RenderUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicSkin"/> class.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="renderUtilities">
        /// The render utilities.
        /// </param>
        /// <param name="assetManagerProvider">
        /// The asset manager provider.
        /// </param>
        public TransparentBasicSkin(
            IBasicSkin skin, 
            I2DRenderUtilities renderUtilities, 
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_BasicSkin = skin;
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
        }

        /// <summary>
        /// Gets the additional menu item width.
        /// </summary>
        /// <value>
        /// The additional menu item width.
        /// </value>
        public int AdditionalMenuItemWidth
        {
            get
            {
                return 20;
            }
        }

        /// <summary>
        /// Gets the height for tree item.
        /// </summary>
        /// <value>
        /// The height for tree item.
        /// </value>
        public int HeightForTreeItem
        {
            get
            {
                return 16;
            }
        }

        /// <summary>
        /// Gets the list horizontal padding.
        /// </summary>
        /// <value>
        /// The list horizontal padding.
        /// </value>
        public int ListHorizontalPadding
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the list vertical padding.
        /// </summary>
        /// <value>
        /// The list vertical padding.
        /// </value>
        public int ListVerticalPadding
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the main menu horizontal padding.
        /// </summary>
        /// <value>
        /// The main menu horizontal padding.
        /// </value>
        public int MainMenuHorizontalPadding
        {
            get
            {
                return 10;
            }
        }

        /// <summary>
        /// Gets the menu item height.
        /// </summary>
        /// <value>
        /// The menu item height.
        /// </value>
        public int MenuItemHeight
        {
            get
            {
                return 24;
            }
        }

        public int HorizontalScrollBarHeight { get { return 16; } }

        public int VerticalScrollBarWidth { get { return 16; } }

        /// <summary>
        /// The draw audio player.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="audioPlayer">
        /// The audio player.
        /// </param>
        public void DrawAudioPlayer(IRenderContext context, Rectangle layout, AudioPlayer audioPlayer)
        {
            this.DrawSunken(context, layout);

            if (audioPlayer.Audio != null && audioPlayer.Audio.PlatformData != null)
            {
                this.m_RenderUtilities.RenderText(
                    context, 
                    new Vector2(layout.Center.X, layout.Center.Y + 12), 
                    "No visualization available.", 
                    this.m_AssetManager.Get<FontAsset>("font.Default"), 
                    HorizontalAlignment.Center, 
                    VerticalAlignment.Center);
            }
        }

        /// <summary>
        /// The draw button.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="button">
        /// The button.
        /// </param>
        public void DrawButton(IRenderContext context, Rectangle layout, Button button)
        {
            var offset = 0;
            if (button.State == ButtonUIState.Clicked)
            {
                this.DrawSunken(context, layout);
                offset = 1;
            }
            else
            {
                this.DrawRaised(context, layout);
            }

            this.m_RenderUtilities.RenderText(
                context, 
                new Vector2(layout.Center.X + offset, layout.Center.Y + offset), 
                button.Text, 
                this.m_AssetManager.Get<FontAsset>("font.Default"), 
                HorizontalAlignment.Center, 
                VerticalAlignment.Center);
        }

        /// <summary>
        /// The draw canvas.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="canvas">
        /// The canvas.
        /// </param>
        public void DrawCanvas(IRenderContext context, Rectangle layout, Canvas canvas)
        {
        }

        /// <summary>
        /// The draw file select.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="fileSelect">
        /// The file select.
        /// </param>
        public void DrawFileSelect(IRenderContext context, Rectangle layout, FileSelect fileSelect)
        {
            var offset = 0;
            if (fileSelect.State == ButtonUIState.Clicked)
            {
                this.DrawSunken(context, layout);
                offset = 1;
            }
            else
            {
                this.DrawRaised(context, layout);
            }

            var text = fileSelect.Path ?? string.Empty;
            while (text.Length > 0 && this.MeasureText(context, "(file select) ..." + text).X > layout.Width - 10)
            {
                text = text.Substring(1);
            }

            if (text.Length != (fileSelect.Path ?? string.Empty).Length)
            {
                text = "..." + text;
            }

            this.m_RenderUtilities.RenderText(
                context, 
                new Vector2(layout.Center.X + offset, layout.Center.Y + offset), 
                "(file select) " + text, 
                this.m_AssetManager.Get<FontAsset>("font.Default"), 
                HorizontalAlignment.Center, 
                VerticalAlignment.Center);
        }

        /// <summary>
        /// The draw fixed container.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="fixedContainer">
        /// The fixed container.
        /// </param>
        public void DrawFixedContainer(IRenderContext context, Rectangle layout, FixedContainer fixedContainer)
        {
        }

        /// <summary>
        /// The draw font viewer.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="fontViewer">
        /// The font viewer.
        /// </param>
        public void DrawFontViewer(IRenderContext context, Rectangle layout, FontViewer fontViewer)
        {
            this.DrawSunken(context, layout);

            if (fontViewer.Font != null && fontViewer.Font.PlatformData != null)
            {
                this.m_RenderUtilities.RenderText(
                    context, 
                    new Vector2(layout.X, layout.Y), 
                    "Font Example", 
                    fontViewer.Font);
            }
        }

        /// <summary>
        /// The draw form.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="form">
        /// The form.
        /// </param>
        public void DrawForm(IRenderContext context, Rectangle layout, Form form)
        {
            this.DrawFlat(context, layout);
        }

        /// <summary>
        /// The draw horizontal container.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="horizontalContainer">
        /// The horizontal container.
        /// </param>
        public void DrawHorizontalContainer(
            IRenderContext context, 
            Rectangle layout, 
            HorizontalContainer horizontalContainer)
        {
        }

        /// <summary>
        /// The draw label.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        public void DrawLabel(IRenderContext context, Rectangle layout, Label label)
        {
            this.m_RenderUtilities.RenderText(
                context, 
                new Vector2(layout.Center.X, layout.Center.Y), 
                label.Text, 
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                HorizontalAlignment.Center, 
                VerticalAlignment.Center,
                label.OverrideColor);
        }

        /// <summary>
        /// The draw link.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="link">
        /// The link.
        /// </param>
        public void DrawLink(IRenderContext context, Rectangle layout, Link link)
        {
            this.m_RenderUtilities.RenderText(
                context, 
                new Vector2(layout.Center.X, layout.Center.Y), 
                link.Text, 
                this.m_AssetManager.Get<FontAsset>("font.Default"), 
                HorizontalAlignment.Center, 
                VerticalAlignment.Center, 
                Color.Blue);
        }

        /// <summary>
        /// The draw list item.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="listItem">
        /// The list item.
        /// </param>
        public void DrawListItem(IRenderContext context, Rectangle layout, ListItem listItem)
        {
            if (listItem.Parent is ListView)
            {
                var view = listItem.Parent as ListView;
                if (view.SelectedItem == listItem)
                {
                    this.DrawRaised(context, layout);
                }
            }

            this.m_RenderUtilities.RenderText(
                context, 
                new Vector2(layout.X + 5, layout.Center.Y), 
                listItem.Text, 
                this.m_AssetManager.Get<FontAsset>("font.Default"), 
                verticalAlignment: VerticalAlignment.Center);
        }

        /// <summary>
        /// The draw list view.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="listView">
        /// The list view.
        /// </param>
        public void DrawListView(IRenderContext context, Rectangle layout, ListView listView)
        {
            this.DrawSunken(context, layout);
        }

        /// <summary>
        /// The draw main menu.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="mainMenu">
        /// The main menu.
        /// </param>
        public void DrawMainMenu(IRenderContext context, Rectangle layout, MainMenu mainMenu)
        {
            this.DrawFlat(context, layout);
        }

        /// <summary>
        /// The draw menu item.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="menuItem">
        /// The menu item.
        /// </param>
        public void DrawMenuItem(IRenderContext context, Rectangle layout, MenuItem menuItem)
        {
            if (menuItem.Active)
            {
                this.DrawRaised(context, layout);
            }
            else
            {
                this.DrawFlat(context, layout);
            }

            this.m_RenderUtilities.RenderText(
                context, 
                new Vector2(layout.X + 5, layout.Center.Y), 
                menuItem.Text, 
                this.m_AssetManager.Get<FontAsset>("font.Default"), 
                verticalAlignment: VerticalAlignment.Center);
        }

        /// <summary>
        /// The draw menu list.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="menuItem">
        /// The menu item.
        /// </param>
        public void DrawMenuList(IRenderContext context, Rectangle layout, MenuItem menuItem)
        {
            this.DrawRaised(context, layout);
        }

        /// <summary>
        /// The draw single container.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="singleContainer">
        /// The single container.
        /// </param>
        public void DrawSingleContainer(IRenderContext context, Rectangle layout, SingleContainer singleContainer)
        {
            this.DrawSunken(context, layout);
        }

        public void DrawScrollableContainer(IRenderContext context, Rectangle layout, ScrollableContainer scrollableContainer, RenderTarget2D childContent)
        {
            this.DrawSunken(context, layout);

            var layoutWidth = layout.Width - this.HorizontalScrollBarHeight;
            var layoutHeight = layout.Height - this.VerticalScrollBarWidth;

            this.m_RenderUtilities.RenderTexture(
                context,
                new Vector2(layout.X, layout.Y),
                new TextureAsset(childContent),
                new Vector2(layoutWidth, layoutHeight),
                sourceArea: new Rectangle(
                    (int)(scrollableContainer.ScrollX * (System.Math.Max(childContent.Width, layoutWidth) - layoutWidth)),
                    (int)(scrollableContainer.ScrollY * (System.Math.Max(childContent.Height, layoutHeight) - layoutHeight)),
                    layoutWidth,
                    layoutHeight));

            var raisedPadding = 3;

            this.DrawSunken(context, new Rectangle(
                layout.X,
                layout.Y + layout.Height - this.HorizontalScrollBarHeight,
                layout.Width - this.VerticalScrollBarWidth,
                this.HorizontalScrollBarHeight));
            this.DrawSunken(context, new Rectangle(
                layout.X + layout.Width - this.VerticalScrollBarWidth,
                layout.Y,
                this.VerticalScrollBarWidth,
                layout.Height - this.HorizontalScrollBarHeight));
            
            this.DrawRaised(context, new Rectangle(
                (int)(layout.X + scrollableContainer.ScrollX * (layoutWidth - ((layoutWidth / (float)childContent.Width) * layoutWidth))) + raisedPadding,
                layout.Y + layout.Height - this.HorizontalScrollBarHeight + raisedPadding,
                (int)((layoutWidth / (float)childContent.Width) * layoutWidth) - raisedPadding * 2,
                this.HorizontalScrollBarHeight - raisedPadding * 2));
            
            this.DrawRaised(context, new Rectangle(
                layout.X + layout.Width - this.VerticalScrollBarWidth + raisedPadding,
                (int)(layout.Y + scrollableContainer.ScrollY * (layoutHeight - ((layoutHeight / (float)childContent.Height) * layoutHeight))) + raisedPadding,
                this.VerticalScrollBarWidth - raisedPadding * 2,
                (int)((layoutHeight / (float)childContent.Height) * layoutHeight) - raisedPadding * 2));
        }

        /// <summary>
        /// The draw text box.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="textBox">
        /// The text box.
        /// </param>
        public void DrawTextBox(IRenderContext context, Rectangle layout, TextBox textBox)
        {
            this.DrawSunken(context, layout);
            var textToRender = textBox.Text;
            if (textBox.Focused && (textBox.UpdateCounter / 15) % 2 == 0)
            {
                textToRender += "_";
            }

            if (string.IsNullOrEmpty(textBox.Text) && !textBox.Focused)
            {
                this.m_RenderUtilities.RenderText(
                    context,
                    new Vector2(layout.X + 4, layout.Center.Y), 
                    textBox.Hint, 
                    this.m_AssetManager.Get<FontAsset>("font.Default"), 
                    textColor: Color.DimGray, 
                    verticalAlignment: VerticalAlignment.Center);
            }
            else
            {
                this.m_RenderUtilities.RenderText(
                    context, 
                    new Vector2(layout.X + 4, layout.Center.Y), 
                    textToRender, 
                    this.m_AssetManager.Get<FontAsset>("font.Default"), 
                    verticalAlignment: VerticalAlignment.Center);
            }
        }

        /// <summary>
        /// The draw texture viewer.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="textureViewer">
        /// The texture viewer.
        /// </param>
        public void DrawTextureViewer(IRenderContext context, Rectangle layout, TextureViewer textureViewer)
        {
            this.DrawSunken(context, layout);

            if (textureViewer.Texture != null && textureViewer.Texture.PlatformData != null)
            {
                this.m_RenderUtilities.RenderTexture(context, new Vector2(layout.X, layout.Y), textureViewer.Texture);
            }
        }

        /// <summary>
        /// The draw tree item.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="treeItem">
        /// The tree item.
        /// </param>
        public void DrawTreeItem(IRenderContext context, Rectangle layout, TreeItem treeItem)
        {
            if (treeItem.Parent is TreeView)
            {
                var view = treeItem.Parent as TreeView;
                if (view.SelectedItem == treeItem)
                {
                    this.DrawRaised(context, layout);
                }
            }

            this.m_RenderUtilities.RenderText(
                context, 
                new Vector2(layout.X + 5, layout.Center.Y), 
                treeItem.Text, 
                this.m_AssetManager.Get<FontAsset>("font.Default"), 
                verticalAlignment: VerticalAlignment.Center);
        }

        /// <summary>
        /// The draw tree view.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="treeView">
        /// The tree view.
        /// </param>
        public void DrawTreeView(IRenderContext context, Rectangle layout, TreeView treeView)
        {
            this.DrawSunken(context, layout);
        }

        /// <summary>
        /// The draw vertical container.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="verticalContainer">
        /// The vertical container.
        /// </param>
        public void DrawVerticalContainer(IRenderContext context, Rectangle layout, VerticalContainer verticalContainer)
        {
        }

        /// <summary>
        /// The draw window.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="window">
        /// The window.
        /// </param>
        public void DrawWindow(IRenderContext context, Rectangle layout, Window window)
        {
            this.DrawRaised(context, layout);
        }

        /// <summary>
        /// The measure text.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/>.
        /// </returns>
        public Vector2 MeasureText(IRenderContext context, string text)
        {
            return this.m_RenderUtilities.MeasureText(context, text, this.m_AssetManager.Get<FontAsset>("font.Default"));
        }

        /// <summary>
        /// The draw flat.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        private void DrawFlat(IRenderContext context, Rectangle layout)
        {
            this.m_RenderUtilities.RenderRectangle(context, layout, this.m_BasicSkin.SurfaceColor, true);
            this.m_RenderUtilities.RenderRectangle(context, layout, this.m_BasicSkin.LightEdgeColor);
        }

        /// <summary>
        /// The draw raised.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        private void DrawRaised(IRenderContext context, Rectangle layout)
        {
            this.m_RenderUtilities.RenderRectangle(context, layout, this.m_BasicSkin.SurfaceColor, true);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X, layout.Y + layout.Height - 1), 
                new Vector2(layout.X + layout.Width, layout.Y + layout.Height - 1), 
                this.m_BasicSkin.DarkEdgeColor);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X + layout.Width - 1, layout.Y), 
                new Vector2(layout.X + layout.Width - 1, layout.Y + layout.Height), 
                this.m_BasicSkin.DarkEdgeColor);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X, layout.Y), 
                new Vector2(layout.X + layout.Width, layout.Y), 
                this.m_BasicSkin.LightEdgeColor);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X, layout.Y), 
                new Vector2(layout.X, layout.Y + layout.Height), 
                this.m_BasicSkin.LightEdgeColor);
        }

        /// <summary>
        /// The draw sunken.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        private void DrawSunken(IRenderContext context, Rectangle layout)
        {
            this.m_RenderUtilities.RenderRectangle(context, layout, this.m_BasicSkin.DarkSurfaceColor);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X, layout.Y + layout.Height - 1), 
                new Vector2(layout.X + layout.Width, layout.Y + layout.Height - 1), 
                this.m_BasicSkin.LightEdgeColor);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X + layout.Width - 1, layout.Y), 
                new Vector2(layout.X + layout.Width - 1, layout.Y + layout.Height), 
                this.m_BasicSkin.LightEdgeColor);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X, layout.Y), 
                new Vector2(layout.X + layout.Width, layout.Y), 
                this.m_BasicSkin.DarkEdgeColor);
            this.m_RenderUtilities.RenderLine(
                context, 
                new Vector2(layout.X, layout.Y), 
                new Vector2(layout.X, layout.Y + layout.Height), 
                this.m_BasicSkin.DarkEdgeColor);
        }

        public void BeforeRenderTargetChange(IRenderContext renderContext)
        {
            this.m_RenderUtilities.SuspendSpriteBatch(renderContext);
        }

        public void AfterRenderTargetChange(IRenderContext renderContext)
        {
            this.m_RenderUtilities.ResumeSpriteBatch(renderContext);
        }
    }
}