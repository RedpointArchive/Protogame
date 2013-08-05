using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Protogame
{
    public class MainMenu : MenuItem
    {
        public MainMenu(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities renderUtilities) : base(assetManagerProvider, renderUtilities)
        {
        }
        
        private IEnumerable<KeyValuePair<MenuItem, Rectangle>> GetChildLocations(ISkin skin, Rectangle layout)
        {
            var accumulated = -skin.MainMenuHorizontalPadding;
            foreach (var child in m_Items)
            {
                yield return new KeyValuePair<MenuItem, Rectangle>(
                    child,
                    new Rectangle(
                        layout.X + accumulated + skin.MainMenuHorizontalPadding,
                        layout.Y,
                        child.TextWidth + skin.MainMenuHorizontalPadding,
                        layout.Height));
                accumulated += child.TextWidth + skin.MainMenuHorizontalPadding;
            }
        }

        public override void Update(ISkin skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in GetChildLocations(skin, layout))
                kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
        }

        public override void Draw(IRenderContext context, ISkin skin, Rectangle layout)
        {
            skin.DrawMainMenu(context, layout, this);
            foreach (var kv in GetChildLocations(skin, layout))
            {
                kv.Key.Draw(context, skin, kv.Value);
            }
        }
    }
}

