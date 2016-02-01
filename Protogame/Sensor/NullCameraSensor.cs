using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class NullCameraSensor : ICameraSensor
    {
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public Texture2D VideoCaptureFrame => null;
        public byte[] VideoCaptureUnlockedRGBA => null;

        public int? VideoCaptureWidth => null;
        public int? VideoCaptureHeight => null;

        public ICamera ActiveCamera
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }

        public List<ICamera> GetAvailableCameras()
        {
            return new List<ICamera>();
        }
    }
}