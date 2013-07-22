using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicSkin : ISkin
    {
        private IBasicSkin m_BasicSkin;
        private IRenderUtilities m_RenderUtilities;
        private IAssetManager m_AssetManager;

        public BasicSkin(
            IBasicSkin skin,
            IRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_BasicSkin = skin;
            this.m_RenderUtilities = renderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager(false);
        }

        private void DrawRaised(IRenderContext context, Rectangle layout)
        {
            this.m_RenderUtilities.RenderRectangle(
                context,
                layout, 
                this.m_BasicSkin.SurfaceColor, 
                filled: true);
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

        private void DrawFlat(IRenderContext context, Rectangle layout)
        {
            this.m_RenderUtilities.RenderRectangle(
                context,
                layout,
                this.m_BasicSkin.SurfaceColor,
                filled: true);
            this.m_RenderUtilities.RenderRectangle(
                context,
                layout,
                this.m_BasicSkin.LightEdgeColor);
        }

        private void DrawSunken(IRenderContext context, Rectangle layout)
        {
            this.m_RenderUtilities.RenderRectangle(
                context,
                layout,
                this.m_BasicSkin.DarkSurfaceColor);
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

        public void DrawButton(IRenderContext context, Rectangle layout, Button button)
        {
            var offset = 0;
            if (button.State == ButtonUIState.Clicked)
            {
                this.DrawSunken(context, layout);
                offset = 1;
            }
            else
                this.DrawRaised(context, layout);
            this.m_RenderUtilities.RenderText(
                context,
                new Vector2(
                    layout.Center.X + offset,
                    layout.Center.Y + offset),
                button.Text,
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                horizontalAlignment: HorizontalAlignment.Center,
                verticalAlignment: VerticalAlignment.Center);
        }

        public void DrawCanvas(IRenderContext context, Rectangle layout, Canvas canvas)
        {
            this.m_RenderUtilities.RenderRectangle(
                context,
                layout,
                this.m_BasicSkin.BackSurfaceColor,
                filled: true);
        }

        public void DrawFixedContainer(IRenderContext context, Rectangle layout, FixedContainer fixedContainer)
        {
        }

        public void DrawLabel(IRenderContext context, Rectangle layout, Label label)
        {
            this.m_RenderUtilities.RenderText(
                context,
                new Vector2(
                    layout.Center.X,
                    layout.Center.Y),
                label.Text,
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                horizontalAlignment: HorizontalAlignment.Center,
                verticalAlignment: VerticalAlignment.Center);
        }

        public void DrawLink(IRenderContext context, Rectangle layout, Link link)
        {
            this.m_RenderUtilities.RenderText(
                context,
                new Vector2(
                    layout.Center.X,
                    layout.Center.Y),
                link.Text,
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                horizontalAlignment: HorizontalAlignment.Center,
                verticalAlignment: VerticalAlignment.Center,
                textColor: Color.Blue);
        }

        public void DrawVerticalContainer(IRenderContext context, Rectangle layout, VerticalContainer verticalContainer)
        {
        }

        public void DrawHorizontalContainer(
            IRenderContext context,
            Rectangle layout,
            HorizontalContainer horizontalContainer)
        {
        }

        public void DrawMenuItem(IRenderContext context, Rectangle layout, MenuItem menuItem)
        {
            if (menuItem.Active)
                this.DrawRaised(context, layout);
            else
                this.DrawFlat(context, layout);
            this.m_RenderUtilities.RenderText(
                context,
                new Vector2(
                    layout.X + 5,
                    layout.Center.Y),
                menuItem.Text,
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                verticalAlignment: VerticalAlignment.Center);
        }

        public void DrawMenuList(IRenderContext context, Rectangle layout, MenuItem menuItem)
        {
            this.DrawRaised(context, layout);
        }

        public void DrawMainMenu(IRenderContext context, Rectangle layout, MainMenu mainMenu)
        {
            this.DrawFlat(context, layout);
        }

        public void DrawTreeView(IRenderContext context, Rectangle layout, TreeView treeView)
        {
            this.DrawSunken(context, layout);
        }

        public void DrawTreeItem(IRenderContext context, Rectangle layout, TreeItem treeItem)
        {
            if (treeItem.Parent is TreeView)
            {
                var view = (treeItem.Parent as TreeView);
                if (view.SelectedItem == treeItem)
                {
                    this.DrawRaised(context, layout);
                }
            }
            this.m_RenderUtilities.RenderText(
                context,
                new Vector2(
                    layout.X + 5,
                    layout.Center.Y),
                treeItem.Text,
                this.m_AssetManager.Get<FontAsset>("font.Default"),
                verticalAlignment: VerticalAlignment.Center);
        }

        public void DrawSingleContainer(IRenderContext context, Rectangle layout, SingleContainer singleContainer)
        {
            this.DrawSunken(context, layout);
        }

        public void DrawTextBox(IRenderContext context, Rectangle layout, TextBox textBox)
        {
            this.DrawSunken(context, layout);
            var textToRender = textBox.Text;
            if (textBox.Focused && (textBox.UpdateCounter / 15) % 2 == 0)
                textToRender += "_";
            if (string.IsNullOrEmpty(textBox.Text) && !textBox.Focused)
                this.m_RenderUtilities.RenderText(
                    context,
                    new Vector2(
                        layout.X,
                        layout.Center.Y),
                    textBox.Hint,
                    this.m_AssetManager.Get<FontAsset>("font.Default"),
                    textColor: Color.DimGray,
                    verticalAlignment: VerticalAlignment.Center);
            else
                this.m_RenderUtilities.RenderText(
                    context,
                    new Vector2(
                        layout.X,
                        layout.Center.Y),
                    textToRender,
                    this.m_AssetManager.Get<FontAsset>("font.Default"),
                    verticalAlignment: VerticalAlignment.Center);
        }
        
        public void DrawForm(IRenderContext context, Rectangle layout, Form form)
        {
            this.DrawFlat(context, layout);
        }
        
        public void DrawFontViewer(IRenderContext context, Rectangle layout, FontViewer fontViewer)
        {
            this.DrawSunken(context, layout);
            
            if (fontViewer.Font != null && fontViewer.Font.FontData != null)
            {
                this.m_RenderUtilities.RenderText(
                    context,
                    new Vector2(
                        layout.X,
                        layout.Y),
                    "Font Example",
                    fontViewer.Font);
            }
        }
        
        public Vector2 MeasureText(IRenderContext context, string text)
        {
            return this.m_RenderUtilities.MeasureText(
                context,
                text,
                this.m_AssetManager.Get<FontAsset>("font.Default"));
        }

        public int HeightForTreeItem
        {
            get { return 16; }
        }

        public int MainMenuHorizontalPadding
        {
            get { return 10; }
        }

        public int AdditionalMenuItemWidth
        {
            get { return 20; }
        }

        public int MenuItemHeight
        {
            get { return 24; }
        }
    }
}

