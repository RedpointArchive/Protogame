#if PLATFORM_WINDOWS

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DirectShowCameraSensor : ICameraSensor
    {
        private VideoCapture _videoCapture;

        private ICamera _currentCamera;
        
        public Texture2D VideoCaptureFrame { get; private set; }

        public byte[] VideoCaptureUnlockedRGBA { get; private set; }

        public int? VideoCaptureWidth
        {
            get
            {
                if (VideoCaptureFrame != null)
                {
                    return VideoCaptureFrame.Width;
                }
                else
                {
                    return null;
                }
            }
        }

        public int? VideoCaptureHeight
        {
            get
            {
                if (VideoCaptureFrame != null)
                {
                    return VideoCaptureFrame.Height;
                }
                else
                {
                    return null;
                }
            }
        }

        public ICamera ActiveCamera { get; set; }

        public List<ICamera> GetAvailableCameras()
        {
            return VideoCapture.GetDirectShowCameras();
        }
        
        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.GraphicsDevice == null)
            {
                return;
            }

            if (_currentCamera != ActiveCamera)
            {
                if (_videoCapture != null)
                {
                    _videoCapture.Dispose();
                }

                _videoCapture = null;
            }

            _currentCamera = ActiveCamera;

            if (_videoCapture == null && _currentCamera != null)
            {
                _videoCapture = new VideoCapture(
                    renderContext.GraphicsDevice,
                    _currentCamera);
            }

            if (_videoCapture != null)
            {
                VideoCaptureFrame = _videoCapture.Frame;
                VideoCaptureUnlockedRGBA = _videoCapture.UnlockedFrameRGBA;
            }
            else
            {
                VideoCaptureFrame = null;
                VideoCaptureUnlockedRGBA = null;
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}

#endif