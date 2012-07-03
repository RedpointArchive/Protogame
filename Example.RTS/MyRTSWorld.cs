using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Protogame.RTS;
using Protogame.MultiLevel;

namespace Example.RTS
{
    public class MyRTSWorld : RTSWorld
    {
        public MyRTSWorld()
            : base()
        {
            this.UiManager.Log("press Q, W, E, R or T to change level view");
        }

        protected override Dictionary<Level, string> GetLevelToTilesetMapping(string name)
        {
            return new Dictionary<Level, string>
            {
                { new SurfaceLevel(this), "Surface" },
                { new MilitaryLevel(this), "Military" },
                { new SmallMilitaryLevel(this), "SmallMil" },
                { new CaveLevel(this), "Cave" },
                { new PocketLevel(this), "Pocket" }
            };
        }
    }
}
