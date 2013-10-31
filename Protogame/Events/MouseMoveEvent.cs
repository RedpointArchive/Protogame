using System;

namespace Protogame
{
    public class MouseMoveEvent : MouseEvent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int LastX { get; set; }
        public int LastY { get; set; }
    }
}

