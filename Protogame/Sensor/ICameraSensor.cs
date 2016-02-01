using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface ICameraSensor : ISensor
    {
        Texture2D VideoCaptureFrame { get; }

        byte[] VideoCaptureUnlockedRGBA { get; }

        int? VideoCaptureWidth { get; }

        int? VideoCaptureHeight { get; }

        ICamera ActiveCamera { get; set; }

        List<ICamera> GetAvailableCameras();
    }
}
