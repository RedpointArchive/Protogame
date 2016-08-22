using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Protogame
{   
    public class MainMenu : MenuItem
    {
        public override void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout)
        {
            skinDelegator.Render(context, layout, this);
            foreach (var kv in GetChildLocations(skinLayout, layout))
            {
                kv.Key.Render(context, skinLayout, skinDelegator, kv.Value);
            }
        }
        
        public override void Update(ISkinLayout skin, Rectangle layout, GameTime gameTime, ref bool stealFocus)
        {
            foreach (var kv in this.GetChildLocations(skin, layout))
            {
                kv.Key.Update(skin, kv.Value, gameTime, ref stealFocus);
            }
        }
        
        private IEnumerable<KeyValuePair<MenuItem, Rectangle>> GetChildLocations(ISkinLayout skin, Rectangle layout)
        {
            var accumulated = -skin.MainMenuHorizontalPadding;
            foreach (var child in this.Items)
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