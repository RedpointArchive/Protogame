namespace Protogame
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The main menu.
    /// </summary>
    public class MainMenu : MenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenu"/> class.
        /// </summary>
        /// <param name="assetManagerProvider">
        /// The asset manager provider.
        /// </param>
        /// <param name="renderUtilities">
        /// The render utilities.
        /// </param>
        public MainMenu(IAssetManagerProvider assetManagerProvider, I2DRenderUtilities renderUtilities)
            : base(assetManagerProvider, renderUtilities)
        {
        }

        /// <summary>
        /// The draw.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        public override void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawMainMenu(context, layout, this);
            foreach (var kv in this.GetChildLocations(skin, layout))
            {
                kv.Key.Draw(context, skin, kv.Value);
            }
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <param name="gameTime">
        /// The game time.
        /// </param>
        /// <param name="stealFocus">
        /// The steal focus.
        /// </param>
        public override void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in this.GetChildLocations(skin, layout))
            {
                kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
            }
        }

        /// <summary>
        /// The get child locations.
        /// </summary>
        /// <param name="skin">
        /// The skin.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<KeyValuePair<MenuItem, Rectangle>> GetChildLocations(ISkin skin, Rectangle layout)
        {
            var accumulated = -skin.MainMenuHorizontalPadding;
            foreach (var child in this.m_Items)
            {
                yield return
                    new KeyValuePair<MenuItem, Rectangle>(
                        child, 
                        new Rectangle(
                            layout.X + accumulated + skin.MainMenuHorizontalPadding, 
                            layout.Y, 
                            child.TextWidth + skin.MainMenuHorizontalPadding, 
                            layout.Height));
                accumulated += child.TextWidth + skin.MainMenuHorizontalPadding;
            }
        }
    }
}