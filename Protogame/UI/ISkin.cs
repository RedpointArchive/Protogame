namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The Skin interface.
    /// </summary>
    public interface ISkin
    {
        /// <summary>
        /// Gets the additional menu item width.
        /// </summary>
        /// <value>
        /// The additional menu item width.
        /// </value>
        int AdditionalMenuItemWidth { get; }

        /// <summary>
        /// Gets the height for tree item.
        /// </summary>
        /// <value>
        /// The height for tree item.
        /// </value>
        int HeightForTreeItem { get; }

        /// <summary>
        /// Gets the list horizontal padding.
        /// </summary>
        /// <value>
        /// The list horizontal padding.
        /// </value>
        int ListHorizontalPadding { get; }

        /// <summary>
        /// Gets the list vertical padding.
        /// </summary>
        /// <value>
        /// The list vertical padding.
        /// </value>
        int ListVerticalPadding { get; }

        /// <summary>
        /// Gets the main menu horizontal padding.
        /// </summary>
        /// <value>
        /// The main menu horizontal padding.
        /// </value>
        int MainMenuHorizontalPadding { get; }

        /// <summary>
        /// Gets the menu item height.
        /// </summary>
        /// <value>
        /// The menu item height.
        /// </value>
        int MenuItemHeight { get; }

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
        void DrawAudioPlayer(IRenderContext context, Rectangle layout, AudioPlayer audioPlayer);

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
        void DrawButton(IRenderContext context, Rectangle layout, Button button);

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
        void DrawCanvas(IRenderContext context, Rectangle layout, Canvas canvas);

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
        void DrawFileSelect(IRenderContext context, Rectangle layout, FileSelect fileSelect);

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
        void DrawFixedContainer(IRenderContext context, Rectangle layout, FixedContainer fixedContainer);

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
        void DrawFontViewer(IRenderContext context, Rectangle layout, FontViewer fontViewer);

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
        void DrawForm(IRenderContext context, Rectangle layout, Form form);

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
        void DrawHorizontalContainer(IRenderContext context, Rectangle layout, HorizontalContainer horizontalContainer);

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
        void DrawLabel(IRenderContext context, Rectangle layout, Label label);

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
        void DrawLink(IRenderContext context, Rectangle layout, Link link);

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
        void DrawListItem(IRenderContext context, Rectangle layout, ListItem listItem);

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
        void DrawListView(IRenderContext context, Rectangle layout, ListView listView);

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
        void DrawMainMenu(IRenderContext context, Rectangle layout, MainMenu mainMenu);

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
        void DrawMenuItem(IRenderContext context, Rectangle layout, MenuItem menuItem);

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
        void DrawMenuList(IRenderContext context, Rectangle layout, MenuItem menuItem);

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
        void DrawSingleContainer(IRenderContext context, Rectangle layout, SingleContainer singleContainer);

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
        void DrawTextBox(IRenderContext context, Rectangle layout, TextBox textBox);

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
        void DrawTextureViewer(IRenderContext context, Rectangle layout, TextureViewer textureViewer);

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
        void DrawTreeItem(IRenderContext context, Rectangle layout, TreeItem treeItem);

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
        void DrawTreeView(IRenderContext context, Rectangle layout, TreeView treeView);

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
        void DrawVerticalContainer(IRenderContext context, Rectangle layout, VerticalContainer verticalContainer);

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
        void DrawWindow(IRenderContext context, Rectangle layout, Window window);

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
        Vector2 MeasureText(IRenderContext context, string text);
    }
}