// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using Microsoft.Xna.Framework.Input;

namespace Protogame
{
    public class ConsoleEventBinder : StaticEventBinder
    {
        public override void Configure()
        {
            this.Bind<Event>(x => true).ToListener<ConsoleEventListener>();
        }
    }
}

